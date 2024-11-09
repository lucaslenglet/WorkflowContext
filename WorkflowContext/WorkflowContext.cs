using CSharpFunctionalExtensions;
using System;
using System.Text.Json.Serialization;

namespace WorkflowContext;

public class WorkflowContext<TValue, TError>
{
    public WorkflowContext(IServiceProvider serviceProvider, TValue value)
    {
        Services = serviceProvider;
        Value = value;
    }

    [JsonIgnore]
    public IServiceProvider Services { get; internal set; }
    [JsonIgnore]
    public UnitResult<TError> Result { get; internal set; } = UnitResult.Success<TError>();

    public TValue Value { get; }
}