using System;
using System.Threading.Tasks;

namespace CommandDotNet.Execution
{
    internal static class InvocationResultExtensions
    {
        internal static async Task<int> GetResultCodeAsync(this object? value)
        {
            switch (value)
            {
                case Task<int> resultCodeTask:
                    return await resultCodeTask.ConfigureAwait(false);
                case Task task:
                    await task.ConfigureAwait(false);
                    return 0;
                case int resultCode:
                    return resultCode;
                case null:
                    return 0;
                default:
                    throw new NotSupportedException($"Unexpected value type: {value} ({value.GetType()})");
            }
        }
    }
}