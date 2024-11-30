using System;
using System.Text.Json.Serialization;

namespace WorkflowContext;

public class WorkflowContext<TData, TError>(IServiceProvider serviceProvider, TData data)
{
    [JsonIgnore]
    public IServiceProvider Services { get; private init; } = serviceProvider;

    [JsonIgnore]
    public WorkflowState<TError> State { get; internal set; } = WorkflowState.Success<TError>();

    public TData Data { get; } = data;

    public void Deconstruct(out TData data, out WorkflowState<TError> state)
    {
        data = Data;
        state = State;
    }

    public void Deconstruct(out TData data, out WorkflowState<TError> state, out IServiceProvider serviceProvider)
    {
        Deconstruct(out data, out state);
        serviceProvider = Services;
    }
}