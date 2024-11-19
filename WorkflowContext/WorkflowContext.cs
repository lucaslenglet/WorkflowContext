using System;
using System.Text.Json.Serialization;

namespace WorkflowContext;

public partial class WorkflowContext<TData, TError>(IServiceProvider serviceProvider, TData data)
{
    [JsonIgnore]
    public IServiceProvider Services { get; internal set; } = serviceProvider;

    [JsonIgnore]
    public WorkflowResult<TError> Result { get; internal set; } = WorkflowResult.Success<TError>();

    public TData Data { get; } = data;

    public void Deconstruct(out TData data, out WorkflowResult<TError> result)
    {
        data = Data;
        result = Result;
    }

    public void Deconstruct(out TData data, out WorkflowResult<TError> result, out IServiceProvider serviceProvider)
    {
        Deconstruct(out data, out result);
        serviceProvider = Services;
    }
}