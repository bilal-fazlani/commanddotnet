using System.Collections.Generic;
using System.Linq;
using CommandDotNet.MicrosoftCommandLineUtils;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.Utils
{
    public static class ArgumentTestExtensions
    {
        public static T ClearValueForTest<T>(this T argumentInfo)
            where T: ArgumentInfo
        {
            SetValueInfo(argumentInfo, CommandOptionType.NoValue);
            return argumentInfo;
        }
        
        public static T SetValueForTest<T>(this T argumentInfo, string value)
            where T: ArgumentInfo
        {
            if (value == null)
            {
                return argumentInfo.ClearValueForTest();
            }
            
            SetValueInfo(argumentInfo, CommandOptionType.SingleValue);
            argumentInfo.ValueInfo.Values = new List<string>() {value};
            return argumentInfo;
        }

        public static T SetValuesForTest<T>(this T argumentInfo, params string[] values)
            where T: ArgumentInfo
        {
            SetValueInfo(argumentInfo, CommandOptionType.MultipleValue);
            argumentInfo.ValueInfo.Values = values.ToList();
            return argumentInfo;
        }

        private static void SetValueInfo(ArgumentInfo argumentInfo, CommandOptionType commandOptionType)
        {
            //set value
            IParameter option = null;

            switch (argumentInfo)
            {
                case CommandOptionInfo optionInfo:
                    option = new CommandOption("--test", commandOptionType);
                    break;
                case CommandParameterInfo parameterInfo:
                    option = new CommandArgument();
                    break;
            }

            argumentInfo.SetValue(option);
        }
    }
}