using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TestTools.Scenarios;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests.Arguments
{
    public class StringCtorObjectsAsArgumentsTests
    {
        private static readonly AppSettings BasicHelp = TestAppSettings.BasicHelp;
        private static readonly AppSettings DetailedHelp = TestAppSettings.DetailedHelp;

        public StringCtorObjectsAsArgumentsTests(ITestOutputHelper output)
        {
            Ambient.Output = output;
        }

        [Fact]
        public void BasicHelp_Includes_StringCtorObjects()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: testhost.dll Do <ctorArg> <parseArg>

Arguments:
  ctorArg
  parseArg"
                }
            });
        }

        [Fact]
        public void BasicHelp_List_Includes_StringCtorObjects()
        {
            new AppRunner<App>(BasicHelp).Verify(new Scenario
            {
                When = {Args = "DoList -h"},
                Then =
                {
                    Output = @"Usage: testhost.dll DoList [options]

Options:
  -c | --ctorArgs
  -p | --parseArgs"
                }
            });
        }

        [Fact]
        public void DetailedHelp_Includes_StringCtorObjects()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "Do -h"},
                Then =
                {
                    Output = @"Usage: testhost.dll Do <ctorArg> <parseArg>

Arguments:

  ctorArg   <FILENAME>

  parseArg  <DIRNAME>"
                }
            });
        }

        [Fact]
        public void DetailedHelp_List_Includes_StringCtorObjects()
        {
            new AppRunner<App>(DetailedHelp).Verify(new Scenario
            {
                When = {Args = "DoList -h"},
                Then =
                {
                    Output = @"Usage: testhost.dll DoList [options]

Options:

  -c | --ctorArgs (Multiple)   <FILENAME>

  -p | --parseArgs (Multiple)  <DIRNAME>"
                }
            });
        }

        [Fact]
        public void Exec_ConvertsStringToObject()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "DoList -c file1 -c file2 -p dir1 -p dir2"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new List<StringCtorObject>
                        {
                            new("file1"),
                            new("file2")
                        },
                        new List<StaticParseObject>
                        {
                            StaticParseObject.Parse("dir1"),
                            StaticParseObject.Parse("dir2")
                        })
                }
            });
        }

        [Fact]
        public void Exec_List_ConvertsStringToObject()
        {
            new AppRunner<App>().Verify(new Scenario
            {
                When = {Args = "Do file1 dir1"},
                Then =
                {
                    AssertContext = ctx => ctx.ParamValuesShouldBe(
                        new StringCtorObject("file1"), StaticParseObject.Parse("dir1"))
                }
            });
        }

        private class App
        {
            public void Do(StringCtorObject ctorArg, StaticParseObject parseArg)
            {
            }

            public void DoList(
                [Option('c')] List<StringCtorObject> ctorArgs,
                [Option('p')] List<StaticParseObject> parseArgs)
            {
            }
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class StringCtorObject
        {
            public string Filename { get; }

            public StringCtorObject(string filename)
            {
                Filename = filename;
            }
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class StaticParseObject
        {
            public string Dirname { get; private set; } = null!;

            public static StaticParseObject Parse(string dirname)
            {
                return new StaticParseObject{Dirname = dirname};
            }
        }
    }
}
