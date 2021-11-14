using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools.Prompts
{
    public static class ConsoleKeyInfos
    {
        public static ConsoleKeyInfo BackSpaceKey = new('\b', ConsoleKey.Backspace, false, false, false);
        public static ConsoleKeyInfo EnterKey = new('\r', ConsoleKey.Enter, false, false, false);
        public static ConsoleKeyInfo CtrlCKey = new('c', ConsoleKey.C, false, false, true);
        public static ConsoleKeyInfo EscapeKey = new(' ', ConsoleKey.Escape, false, false, false);

        public static IEnumerable<ConsoleKeyInfo> AppendEnterKey(this IEnumerable<ConsoleKeyInfo> consoleKeyInfos) =>
            consoleKeyInfos.Concat(EnterKey.ToEnumerable());

        public static IEnumerable<ConsoleKeyInfo> AppendCtrlCKey(this IEnumerable<ConsoleKeyInfo> consoleKeyInfos) =>
            consoleKeyInfos.Concat(CtrlCKey.ToEnumerable());

        public static IEnumerable<ConsoleKeyInfo> AppendEscapeKey(this IEnumerable<ConsoleKeyInfo> consoleKeyInfos) =>
            consoleKeyInfos.Concat(EscapeKey.ToEnumerable());
    }
}