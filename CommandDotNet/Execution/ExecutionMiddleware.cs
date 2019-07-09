using System;

namespace CommandDotNet.Execution
{
    public delegate int ExecutionMiddleware(
        ExecutionContext context,
        Func<ExecutionContext, int> next);
}