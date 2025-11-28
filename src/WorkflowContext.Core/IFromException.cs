using System;

namespace WorkflowContext.Core;

public interface IFromException<TSelf> : IFrom<Exception, TSelf>
    where TSelf : IFrom<Exception, TSelf>;
