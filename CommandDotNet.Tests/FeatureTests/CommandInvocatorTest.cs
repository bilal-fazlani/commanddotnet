using System;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.TestTools;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests.FeatureTests
{
    public class CommandInvokerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CommandInvokerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CanReadAndModifyParamValues()
        {
            Task<int> BeforeInvocation(CommandContext context, Func<CommandContext, Task<int>> next)
            {
                var values = context.InvocationContext.CommandInvocation.ParameterValues;
                values.Length.Should().Be(2);
                var invokedCar = (Car) values[0];
                var invokedOwner = (string)values[1];

                invokedCar.Number.Should().Be(1);
                invokedCar.Number = 2;
                invokedOwner.Should().Be("Jack");
                values[1] = "Jill";

                return next(context);
            }

            var result = RunInMem(1, "Jack", BeforeInvocation);

            result.ExitCode.Should().Be(5);
            result.TestOutputs.Get<Car>().Number.Should().Be(2);
            result.TestOutputs.Get<string>().Should().Be("Jill");
        }

        [Fact]
        public void CanReadAndModifyArgumentValues()
        {
            Task<int> BeforeSetValues(CommandContext context, Func<CommandContext, Task<int>> next)
            {
                var args = context.InvocationContext.CommandInvocation.Arguments;
                args.Count.Should().Be(2);
                var carNumber = args.First();
                var ownerName = args.Last();

                carNumber.Name.Should().Be(nameof(Car.Number));
                ownerName.Name.Should().Be("owner");

                var resultArgumentValues = context.ParseResult.ArgumentValues;

                resultArgumentValues.TryGetValues(carNumber, out var carNumberValues).Should().BeTrue();
                carNumberValues.Single().Should().Be("1");

                resultArgumentValues.TryGetValues(ownerName, out var ownerNameValues).Should().BeTrue();
                ownerNameValues.Single().Should().Be("Jack");
                ownerNameValues[0] = "Jill";

                return next(context);
            }

            var result = RunInMem(1, "Jack", preBindValues: BeforeSetValues);

            result.ExitCode.Should().Be(5);
            result.TestOutputs.Get<Car>().Number.Should().Be(1);
            result.TestOutputs.Get<string>().Should().Be("Jill");
        }

        [Fact]
        public void CanReadCurrentCommand()
        {
            Task<int> BeforeInvocation(CommandContext context, Func<CommandContext, Task<int>> next)
            {
                context.ParseResult.TargetCommand.Should().NotBeNull();
                context.ParseResult.TargetCommand.Name.Should().Be(nameof(App.NotifyOwner));
                return next(context);
            }

            var result = RunInMem(1, "Jack", BeforeInvocation);
        }

        [Fact]
        public void CanReadAndActOnInstance()
        {
            var guid = Guid.NewGuid();

            Task<int> BeforeInvocation(CommandContext context, Func<CommandContext, Task<int>> next)
            {
                var instance = context.InvocationContext.Instance;
                instance.Should().NotBeNull();
                var app = (App)instance;

                app.TestOutputs.Capture(guid);
                return next(context);
            }

            var result = RunInMem(1, "Jack", BeforeInvocation);
            result.TestOutputs.Get<Guid>().Should().Be(guid);
        }

        private AppRunnerResult RunInMem(int carNumber, string ownerName, 
            ExecutionMiddleware postBindValues = null, 
            ExecutionMiddleware preBindValues = null)
        {
            var appRunner = new AppRunner<App>();

            if (postBindValues != null)
            {
                appRunner.Configure(c => c.UseMiddleware(postBindValues, MiddlewareStages.PostBindValuesPreInvoke, int.MaxValue));
            }
            if (preBindValues != null)
            {
                appRunner.Configure(c => c.UseMiddleware(preBindValues, MiddlewareStages.PostParseInputPreBindValues, int.MaxValue));
            }

            var args = $"NotifyOwner --Number {carNumber} --owner {ownerName}".SplitArgs();
            return appRunner.RunInMem(args, _testOutputHelper);
        }
        
        public class App
        {
            [InjectProperty]
            public TestOutputs TestOutputs { get; set; }

            public int NotifyOwner(Car car, [Option] string owner)
            {
                TestOutputs.Capture(car);
                TestOutputs.Capture(owner);
                return 5;
            }
        }

        public class Car : IArgumentModel
        {
            [Option]
            public int Number { get; set; }
        }
    }
}
