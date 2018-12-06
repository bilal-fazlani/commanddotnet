using System.Reflection;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;
using FluentAssertions;
using Xunit;

namespace CommandDotNet.Tests
{
    public class ValueMachineTests
    {
        [Fact]
        public void CanGiveValueWhenUserHasProvidedValue()
        {
            AppSettings appSettings = new AppSettings();
                        
            ValueMachine valueMachine = new ValueMachine(appSettings);
            
            ParameterInfo parameterInfo =
                typeof(TestSubject).GetMethod("TestMethodWithNoDefaultValue").GetParameters()[0];

            CommandOptionInfo optionInfo = new CommandOptionInfo(parameterInfo, appSettings);
            
            optionInfo.SetValue(new CommandOption("--test", CommandOptionType.SingleValue));
            
            optionInfo.ValueInfo.Values.Add("7");
            
            object value = valueMachine.GetValue(optionInfo);

            value.Should().Be(7);
        }
        
        [Fact]
        public void CanGiveValueWhenUserHasProvidedValueAndThereIsADefaultValue()
        {
            AppSettings appSettings = new AppSettings();
                        
            ValueMachine valueMachine = new ValueMachine(appSettings);
            
            ParameterInfo parameterInfo =
                typeof(TestSubject).GetMethod("TestMethodWithDefaultValue").GetParameters()[0];

            CommandOptionInfo optionInfo = new CommandOptionInfo(parameterInfo, appSettings);
            
            optionInfo.SetValue(new CommandOption("--test", CommandOptionType.SingleValue));
            
            optionInfo.ValueInfo.Values.Add("7");
            
            object value = valueMachine.GetValue(optionInfo);

            value.Should().Be(7);
        }

        [Fact]
        public void CanGiveValueWhenUserHasNotProvidedValueAndThereIsADefaultValue()
        {
            AppSettings appSettings = new AppSettings();
                        
            ValueMachine valueMachine = new ValueMachine(appSettings);
            
            ParameterInfo parameterInfo =
                typeof(TestSubject).GetMethod("TestMethodWithDefaultValue").GetParameters()[0];

            CommandOptionInfo optionInfo = new CommandOptionInfo(parameterInfo, appSettings);
            
            optionInfo.SetValue(new CommandOption("--test", CommandOptionType.SingleValue));
            
            object value = valueMachine.GetValue(optionInfo);

            value.Should().Be(5);
        }
        
        [Fact]
        public void CanGiveValueWhenUserHasNotProvidedValueAndThereIsNoDefaultValue()
        {
            AppSettings appSettings = new AppSettings();
                        
            ValueMachine valueMachine = new ValueMachine(appSettings);
            
            
            ParameterInfo parameterInfo =
                typeof(TestSubject).GetMethod("TestMethodWithNoDefaultValue").GetParameters()[0];

            CommandOptionInfo optionInfo = new CommandOptionInfo(parameterInfo, appSettings);
            
            optionInfo.SetValue(new CommandOption("--test", CommandOptionType.SingleValue));
            
            object actualValue = valueMachine.GetValue(optionInfo);

            actualValue.Should().Be(0);
        }
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