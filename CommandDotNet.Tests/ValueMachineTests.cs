using System.Linq;
using CommandDotNet.Models;
using CommandDotNet.Tests.Utils;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests
{
    public class ValueMachineTests
    {
        [Fact]
        public void CanGiveValueWhenUserHasProvidedValue()
        {
            var value = GetValue(nameof(TestSubject.TestMethodWithNoDefaultValue), "7");

            value.Should().Be(7);
        }

        [Fact]
        public void CanGiveValueWhenUserHasProvidedValueAndThereIsADefaultValue()
        {
            var value = GetValue(nameof(TestSubject.TestMethodWithDefaultValue), "7");
            value.Should().Be(7);
        }

        [Fact]
        public void CanGiveValueWhenUserHasNotProvidedValueAndThereIsADefaultValue()
        {
            var value = GetValue(nameof(TestSubject.TestMethodWithDefaultValue), null);
            value.Should().Be(5);
        }
        
        [Fact]
        public void CanGiveValueWhenUserHasNotProvidedValueAndThereIsNoDefaultValue()
        {
            var value = GetValue(nameof(TestSubject.TestMethodWithNoDefaultValue), null);
            value.Should().Be(0);
        }

        private static object GetValue(string methodName, string argValue)
        {
            var appSettings = new AppSettings();
            var valueMachine = new ValueMachine(appSettings);
            
            var optionInfo = TestFactory
                .GetArgumentsFromMethod<TestSubject>(methodName)
                .Cast<CommandOptionInfo>()
                .Single();
            
            optionInfo.SetValueForTest(argValue);

            return valueMachine.GetValue(optionInfo);
        }

        internal class TestSubject
        {
            public void TestMethodWithNoDefaultValue(int value)
            {
            
            }
        
            public void TestMethodWithDefaultValue(int value = 5)
            {
            
            }
        }
    }
}