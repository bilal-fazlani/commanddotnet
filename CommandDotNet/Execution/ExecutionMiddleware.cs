using System;
using System.Threading.Tasks;

namespace CommandDotNet.Execution
{
    public delegate Task<int> ExecutionMiddleware(
        CommandContext context,
        Func<CommandContext, Task<int>> next);
}