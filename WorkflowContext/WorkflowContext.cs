using CSharpFunctionalExtensions;
using System;
using System.Text.Json.Serialization;

namespace WorkflowContext;

public class WorkflowContext<TData, TError>(IServiceProvider serviceProvider, TData data)
{
    [JsonIgnore]
    public IServiceProvider Services { get; internal set; } = serviceProvider;

    [JsonIgnore]
    public UnitResult<TError> Result { get; internal set; } = UnitResult.Success<TError>();

    public TData Data { get; } = data;
}