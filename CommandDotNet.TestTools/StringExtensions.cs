using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Tokens;
using JetBrains.Annotations;

namespace CommandDotNet.TestTools;

[PublicAPI]
public static class StringExtensions
{
    /// <summary>Split the arguments using <see cref="CommandLineStringSplitter"/></summary>
    public static string[] SplitArgs(this string args) =>
        args.IsNullOrWhitespace()
            ? []
            : CommandLineStringSplitter.Instance.Split(args).ToArray();

    public static IEnumerable<ConsoleKeyInfo> ToConsoleKeyInfos(this string? text) =>
        // "\r\n" would result in two ConsoleKey.Enter
        text?.Replace("\r\n", "\r").Select(ToConsoleKeyInfo) ?? EmptyCollection<ConsoleKeyInfo>.Instance;

    public static ConsoleKeyInfo ToConsoleKeyInfo(this char c) => 
        new(c, c.ToConsoleKey(), false, false, false);

    /// <summary>
    /// Convert the char to a <see cref="ConsoleKey"/>.
    /// Defaults to <see cref="ConsoleKey.Oem1"/> if a key is not found<br/>
    /// This is does not an exhaustive list of special cases.<br/>
    /// ' ' <see cref="ConsoleKey.Spacebar"/><br/>
    /// '+' <see cref="ConsoleKey.Add"/><br/>
    /// '-' <see cref="ConsoleKey.Subtract"/><br/>
    /// '*' <see cref="ConsoleKey.Multiply"/><br/>
    /// '/' <see cref="ConsoleKey.Divide"/><br/>
    /// '\b' <see cref="ConsoleKey.Backspace"/><br/>
    /// '\t' <see cref="ConsoleKey.Tab"/><br/>
    /// '\n' <see cref="ConsoleKey.Enter"/><br/>
    /// '\r' <see cref="ConsoleKey.Enter"/><br/>
    /// </summary>
    public static ConsoleKey ToConsoleKey(this char c) =>
        c switch
        {
            ' ' => ConsoleKey.Spacebar,
            '+' => ConsoleKey.Add,
            '-' => ConsoleKey.Subtract,
            '*' => ConsoleKey.Multiply,
            '/' => ConsoleKey.Divide,
            '\b' => ConsoleKey.Backspace,
            '\t' => ConsoleKey.Tab,
            '\n' => ConsoleKey.Enter,
            '\r' => ConsoleKey.Enter,
            _ => char.IsNumber(c) ? $"D{c}".ParseEnum<ConsoleKey>(true) :
                c.ToString().TryParseEnum<ConsoleKey>(out var result, true) ? result : ConsoleKey.Oem1
        };

    internal static T ParseEnum<T>(this string text, bool ignoreCase = false) =>
        (T) Enum.Parse(typeof(T), text, ignoreCase);

    internal static bool TryParseEnum<T>(this string text, out T result, bool ignoreCase = false) where T : struct =>
        Enum.TryParse(text, ignoreCase, out result);
}