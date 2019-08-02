using CommandDotNet.Parsing;

namespace CommandDotNet
{
    /// <summary>Extensions to enable and configure features</summary>
    public static class AppRunnerConfigExtensions
    {
        /// <summary>Enables piped input to be appended to any commands operand list</summary>
        public static AppRunner AppendPipedInputToOperandList(this AppRunner appRunner)
        {
            return PipedInputMiddleware.EnablePipedInput(appRunner);
        }
    }
}