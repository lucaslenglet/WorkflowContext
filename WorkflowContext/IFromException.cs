using System;

namespace WorkflowContext;

public interface IFromException<TSelf> : IFrom<Exception, TSelf>
    where TSelf : IFrom<Exception, TSelf>;
