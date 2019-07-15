using System;
using System.Threading.Tasks;

namespace CommandDotNet.Execution
{
    internal static class InvocationResultExtensions
    {
        internal static async Task<int> GetResultCodeAsync(this object value, CommandContext commandContext)
        {
            switch (value)
            {
                case Task<int> resultCodeTask:
                    return await resultCodeTask;
                case Task task:
                    await task;
                    return 0;
                case int resultCode:
                    return resultCode;
                case null:
                    return 0;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}