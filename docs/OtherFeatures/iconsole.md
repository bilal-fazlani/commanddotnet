# IConsole

CommandDotNet comes with a fairly comprehensive wrapper for System.Console, named SystemConsole.

SystemConsole implements IConsole which can be included in any command or interceptor method.

This makes it easy to test your command input and output. Also makig it easier to test is the [TestConsole](../TestTools/overview.md#testconsole)
described in the TestTools overview.

SystemConsole and TestConsole can be inherited for simpler adaptation resiliant to breakimg changes in IConsole if new members are added.
