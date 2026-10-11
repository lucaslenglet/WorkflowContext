# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
dotnet build                      # whole solution, 0 warnings is the expected state
dotnet test                       # xunit
dotnet test --filter "OnFailure_CanRecover_AndThePipelineResumes"   # one test by name
dotnet run --project samples/WorkflowContext.Sample   # five deterministic order scenarios
dotnet pack src/WorkflowContext -o artifacts          # also runs package validation
```

`TreatWarningsAsErrors` is on repo-wide, and the library sets `GenerateDocumentationFile`: a public
member without an XML doc comment fails the build (CS1591).

The library targets `net8.0;net10.0`; the tests target `net10.0` only, because the dev machine has no
.NET 8 runtime. The net8.0 build is still compiled on every `dotnet build`.

## Pipeline semantics

The operators are the product. Their rules are deliberate decisions, covered by tests in
`WorkflowContextExtensionsTests`:

- **Only `OnFailure` replaces a failed state.** `Execute` / `ExecuteTry` always run but go through
  `WorkflowContext.Apply`, which ignores their result once the context has failed, so a logging or
  audit step can never hide an error. `IfSuccess*` only run on success; `OnFailure*` only on failure,
  and its result replaces the state (return a success to recover, or `ctx.State` to keep the error).
  Their `when` overloads (a predicate on the error) leave non-matching failures untouched; the `when`
  of `IfSuccess*` is a condition on the context, and a false one leaves the state as it is.
- **Errors are single values.** A pipeline stops at the first failure; `IfSuccessAll*` is the one
  place that runs every step and merges their errors, through the opt-in `ICombine<TError>`. Turning
  `WorkflowState` into a list of errors was considered and rejected: it would burden every user and
  force a redefinition of every operator's rule. When an `OnFailureTry` step throws,
  the error from that exception replaces the original one, consistent with "the result replaces".
- **No `Finally` / `Tap` / retry operator.** `Execute` already plays that role for failures. A `Finally` that runs
  after an exception is impossible in this eager model: each operator runs immediately, and once a
  step throws, the context is unreachable from the faulted task. It would need a deferred pipeline
  (`.RunAsync()`), which is a redesign. Retrying belongs inside the step (e.g. Polly) for the same
  reason: once a step has failed, the pipeline no longer knows which one it was.
- **Cancellation belongs to the user.** The context carries a `CancellationToken`; the operators
  never check it. The `Try` operators pass `OperationCanceledException` to `TError.From` like any
  exception, so the error type decides how to represent it; the non-`Try` operators let it propagate.
- **Every `await` uses `ConfigureAwait(false)`.** Steps after an async one may run on a thread-pool
  thread; this is documented on `WorkflowContextExtensions`.

Each operator exists as 4 overloads: {sync, async step} × {context, `Task<context>` receiver}, doubled
by the `when` overloads of `OnFailure*`. They all funnel through the helpers in
`WorkflowContextExtensions.Internal.cs` (`RunIfInternal`, `ReplaceState`, `ReplaceStateTry`); a new
operator follows the same shape.

## Design decisions to keep

- **The static extension class is `WorkflowContextExtensions`, not `WorkflowContext`.** A non-generic
  type named like the `WorkflowContext` namespace breaks name resolution for consumers. The generic
  `WorkflowContext<,>` is safe: a namespace never matches a name with type arguments.
- **Steps resolve dependencies from `ctx.Services`** (service locator). This is chosen, not a smell
  to fix: it keeps steps plain static methods, reusable across workflows (see `AuditSteps` in the
  sample).
- **`IWorkflowContextFactory` is scoped**, so contexts resolve services from the current scope. It
  replaced a three-stage builder (`WithError().WithData().Build()`); keep creation to a single call.
- **The default `WorkflowState<TError>` is a success.** `Error` throws on a success and `Failure`
  rejects null, so `Error` is non-null whenever it is readable.
- **The library depends only on `Microsoft.Extensions.DependencyInjection.Abstractions`**, pinned at
  the lowest version it needs (8.0.2) so consumers stay free to bring a newer one.

A lambda that only throws is ambiguous between the sync and async overloads (CS0121); tests use a
typed local function instead.

## MSBuild layout

- `Directory.Build.props`: shared compiler settings, and `WorkflowContextVersion`, the single source
  for the package version (`0.1.0-local.1` locally).
- `Directory.Packages.props`: central package management.
- The package id is `lucaslgt.WorkflowContext`; namespaces stay `WorkflowContext`.

## Release

Pushing a `v*` tag runs `.github/workflows/release.yml`: the tag is the only source of the published
version (`v1.2.3` -> `1.2.3`), passed as `-p:WorkflowContextVersion=`, so nothing in the repo is
bumped to release. Pushing the `.nupkg` also pushes the `.snupkg` beside it.

Publishing uses nuget.org trusted publishing (OIDC), not a stored API key. The job needs
`id-token: write`, and the `NuGet/login@v1` step must stay immediately before the push: it trades one
OIDC token for one API key valid for a single hour. The only repository secret is `NUGET_USER`.
The GitHub Release is created last, so it exists only for a version that reached nuget.org.

Dependabot ignores `Microsoft.Extensions.DependencyInjection.Abstractions`: its 8.0.2 pin is deliberate
(see above), so an update proposal for it is to be declined.

## Contributions

The repository accepts no pull requests (see `CONTRIBUTING.md`); changes come from the author only.
