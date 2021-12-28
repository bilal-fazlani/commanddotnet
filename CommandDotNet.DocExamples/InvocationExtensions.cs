using System;
using System.IO;
using System.Reflection;
using CommandDotNet.Rendering;
using CommandDotNet.Tokens;
using NUnit.Framework;

namespace CommandDotNet.DocExamples
{
    public static class InvocationExtensions
    {
        /// <summary>
        /// This method exists to validate documentation for cases where config happens external to AppRunner.Run
        /// </summary>
        public static (int exitCode, string output) InvokeMainMethod(this Type type, string args)
        {
            string[] argsArray = new CommandLineStringSplitter().SplitToArray(args);
            var mainMethod = type.GetMethod("Main", BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(mainMethod);

            return CaptureConsoleWrites(() => (int)mainMethod!.Invoke(null, new object[] { argsArray })!);
        }

        private static (int exitCode, string output) CaptureConsoleWrites(Func<int> function)
        {
            using var writer = new StringWriter();
            var @out = new DuplexTextWriter(Console.Out, writer);
            var error = new DuplexTextWriter(Console.Error, writer);
            Console.SetOut(@out);
            Console.SetError(error);
            
            try
            {
                return (function(), writer.ToString());
            }
            finally
            {
                Console.SetOut(@out.Original);
                Console.SetError(error.Original);
            }
        }
    }
}