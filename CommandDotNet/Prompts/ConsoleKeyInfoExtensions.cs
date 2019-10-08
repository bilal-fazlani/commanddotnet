using System;

namespace CommandDotNet.Prompts
{
    internal static class ConsoleKeyInfoExtensions
    {
        internal static bool IsCtrlC(this ConsoleKeyInfo key)
        {
            return key.Key == ConsoleKey.C && key.Modifiers.HasFlag(ConsoleModifiers.Control);
        }
    }
}