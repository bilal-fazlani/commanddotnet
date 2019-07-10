using System;

namespace CommandDotNet.Execution
{
    public delegate int ExecutionMiddleware(
        CommandContext context,
        Func<CommandContext, int> next);
}