using System;

namespace CommandDotNet.Directives
{
    internal class DirectivePipeline
    {
        public static int ProcessDirectives(ExecutionResult executionResult, Func<ExecutionResult, int> next)
        {
            if (executionResult.AppSettings.EnableDirectives)
            {
                DebugDirective.Execute(executionResult, r => 0);
                ParseDirective.Execute(executionResult, r => 0);
            }

            return executionResult.ShouldExit 
                ? executionResult.ExitCode 
                : next(executionResult);
        }
    }
}