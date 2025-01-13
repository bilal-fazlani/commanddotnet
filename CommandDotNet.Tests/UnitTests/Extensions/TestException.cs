using System;
using System.Runtime.CompilerServices;
using CommandDotNet.Diagnostics;
using CommandDotNet.Tokens;

namespace CommandDotNet.Tests.UnitTests.Extensions;

public class TestException : Exception
{
    private static TestException? instance;
            
    public string SomeProperty => "Some property value";

    public static TestException Instance => instance ??= Build();

    public TestException() : base("I'm a test exception")
    {
        base.Data.Add("data-key", "data value");
        base.Data.Add("non-serializable-key", new NonSerializableWrapper("non-serializable-value"));
        this.SetCommandContext(new CommandContext(
            ["does", "not", "matter"], 
            new TokenCollection([]),
            new AppConfigBuilder(new AppSettings()).Build()));
    }

    private static TestException Build()
    {
        try
        {
            ThrowEx();
                
            // this line is never called but required for compiler
            return new TestException();
        }
        catch (TestException e)
        {
            return e;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowEx()
    {
        throw new TestException();
    }
}