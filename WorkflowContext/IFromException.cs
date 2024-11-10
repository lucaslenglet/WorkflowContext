using System;

namespace WorkflowContext;

public interface IFromException<TTo> : IFrom<Exception, TTo>
    where TTo : IFrom<Exception, TTo>;
