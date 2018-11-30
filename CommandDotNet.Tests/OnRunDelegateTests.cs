using System;
using System.Collections.Generic;
using System.Text;
using CommandDotNet.Attributes;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace CommandDotNet.Tests
{
    public class OnRunDelegateTests : TestBase
    {
        public OnRunDelegateTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        [Fact]
        public void CanConstructorsReadModels()
        {
            var testResult = 1;
            var carNumber = "DEY-7776";
            var ownerName = "John";
            AppRunner<OnRunDelegateApp> appRunner = new AppRunner<OnRunDelegateApp>(new AppSettings
            {
                OnRun = context =>
                {
                    context.MergedParameters.Length.Should().Be(2);
                    var car = ((Car)context.MergedParameters[0]);
                    car.Number.Should().Be(carNumber);
                    ((string)context.MergedParameters[1]).Should().Be(ownerName);
                    var realResult = context.RunDelegate(context.MergedParameters);
                    realResult.Should().Be(5);
                    return testResult;
                }
            });
            appRunner.Run("NotifyOwner", "--Number", carNumber, "--owner", ownerName).Should().Be(testResult);
        }

    }

    public class OnRunDelegateApp
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
