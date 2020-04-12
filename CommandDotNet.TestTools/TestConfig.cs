using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools
{
    public class TestConfig
    {
        // TODO: look for CommandDotNet.TestTools.json file

        private static TestConfig s_default;

        public static TestConfig Default
        {
            get => s_default ?? (s_default = GetDefaultFromSubClass() ?? new TestConfig());
            set => s_default = value;
        }

        public static TestConfig Silent { get; set; } = new TestConfig
        {
            OnSuccess = new OnSuccessConfig(), 
            OnError = new OnErrorConfig { CaptureAndReturnResult = true }
        };

        public OnSuccessConfig OnSuccess { get; set; } = new OnSuccessConfig();

        public OnErrorConfig OnError { get; set; } = new OnErrorConfig
        {
            Print = { ConsoleOutput = true }
        };

        public bool PrintCommandDotNetLogs { get; set; }

        /// <summary>
        /// To help determine if the <see cref="TestConfig"/> is the expected default.
        /// The value will print on error.
        /// </summary>
        public string Source { get; set; }

        public class OnSuccessConfig
        {
            public PrintConfig Print { get; set; } = new PrintConfig();
        }

        public class OnErrorConfig
        {
            /// <summary>When true, instead of bubbling up, the error</summary>
            public bool CaptureAndReturnResult { get; set; }
            public PrintConfig Print { get; set; } = new PrintConfig();
        }

        public class PrintConfig
        {
            public bool All
            {
                get => AppConfig && CommandContext && ConsoleOutput && ParseReport;
                set => AppConfig = CommandContext = ConsoleOutput = ParseReport = value;
            }
            public bool AppConfig { get; set; }
            public bool CommandContext { get; set; }
            public bool ConsoleOutput { get; set; }
            public bool ParseReport { get; set; }
        }

        public TestConfig Where(Action<TestConfig> alter)
        {
            if (alter == null)
            {
                throw new ArgumentNullException(nameof(alter));
            }

            var config = new TestConfig
            {
                PrintCommandDotNetLogs = this.PrintCommandDotNetLogs,
                Source = this.Source,
                OnSuccess =
                {
                    Print =
                    {
                        AppConfig = this.OnSuccess.Print.AppConfig,
                        CommandContext = this.OnSuccess.Print.CommandContext,
                        ConsoleOutput = this.OnSuccess.Print.ConsoleOutput,
                        ParseReport = this.OnSuccess.Print.ParseReport
                    }
                },
                OnError =
                {
                    CaptureAndReturnResult = this.OnError.CaptureAndReturnResult,
                    Print =
                    {
                        AppConfig = this.OnError.Print.AppConfig,
                        CommandContext = this.OnError.Print.CommandContext,
                        ConsoleOutput = this.OnError.Print.ConsoleOutput,
                        ParseReport = this.OnError.Print.ParseReport
                    }
                }
            };
            alter(config);
            return config;
        }

        private static TestConfig GetDefaultFromSubClass()
        {
            try
            {
                var assemblies = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => !a.IsDynamic);

                var types = assemblies
                    .SelectMany(assembly => assembly.ExportedTypes)
                    .Where(t => t.InheritsFrom<TestConfig>() && t != typeof(TestConfig))
                    .ToList();
                if (!types.Any())
                {
                    return null;
                }
                if (types.Count > 1)
                {
                    Console.WriteLine(
                        $"Found more than one {nameof(TestConfig)} subclass. Using the first one: {types.ToCsv()}");
                }
                var type = types.First();

                var props = type
                    .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .Where(p => p.CanRead && p.PropertyType.InheritsFrom<TestConfig>()).ToList();
                if (!props.Any())
                {
                    Console.WriteLine($"No public static readable properties of type {nameof(TestConfig)} found in {type}");
                    return null;
                }
                if (props.Count > 1)
                {
                    Console.WriteLine($"Found more than one {nameof(TestConfig)} property in {type}. " +
                                      $"Using the first one: {props.Select(p => p.Name).ToCsv()}");
                }
                var prop = props.First();

                var testConfig = (TestConfig)prop.GetValue(null);
                if (testConfig.Source.IsNullOrWhitespace())
                {
                    testConfig.Source = prop.FullName(includeNamespace: true);
                }
                return testConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to load default test configs:{Environment.NewLine}{e}");
                return null;
            }
        }
    }
}