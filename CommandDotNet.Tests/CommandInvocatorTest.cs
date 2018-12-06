using System;
using CommandDotNet.Attributes;
using CommandDotNet.CommandInvoker;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class CommandInvokerTests : TestBase
    {
        public CommandInvokerTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanConstructorsReadModels()
        {
            string carNumber = "DEY-7776";
            string ownerName = "John";

            Action<CommandInvocation> actionBeforeInvokation =new Action<CommandInvocation>(  context =>
             {
                 context.ParamsForCommandMethod.Length.Should().Be(2);
                 var car = ((Car)context.ParamsForCommandMethod[0]);
                 car.Number.Should().Be(carNumber);
                 ((string)context.ParamsForCommandMethod[1]).Should().Be(ownerName);
             });


            AppRunner<CustomCommandInvokerApp> appRunner = new AppRunner<CustomCommandInvokerApp>().WithCommandInvoker(inner => new CustomCommandInvoker(inner, actionBeforeInvokation));
            appRunner.Run("NotifyOwner", "--Number", carNumber, "--owner", ownerName).Should().Be(5);
        }
    }

    public class CustomCommandInvoker : ICommandInvoker
    {
        private readonly ICommandInvoker _inner;
        private readonly Action<CommandInvocation> _actionBeforeInvokation;

        public CustomCommandInvoker(ICommandInvoker inner, Action<CommandInvocation> actionBeforeInvokation)
        {
            _inner = inner;
            _actionBeforeInvokation = actionBeforeInvokation;
        }

        public object Invoke(CommandInvocation commandInvocation)
        {
            _actionBeforeInvokation(commandInvocation);
            return _inner.Invoke(commandInvocation);
        }
    }

    public class CustomCommandInvokerApp
    {
        public int NotifyOwner(
            Car car,
            [Option(LongName = "owner")]
            string owner)
        {

            return 5;
        }
    }

    public class Car : IArgumentModel
    {
        public int Id { get; set; }

        [Option]
        public string Number { get; set; }
    }
}
