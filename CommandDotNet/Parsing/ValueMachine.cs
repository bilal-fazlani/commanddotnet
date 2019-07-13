using CommandDotNet.ClassModeling;
using CommandDotNet.Extensions;

namespace CommandDotNet.Parsing
{
    internal class ValueMachine
    {
        private readonly AppSettings _appSettings;
        private readonly ParserFactory _parserFactory;

        public ValueMachine(AppSettings appSettings)
        {
            _appSettings = appSettings;
            _parserFactory = new ParserFactory(_appSettings);
        }

        public object GetValue(ArgumentInfo argumentInfo)
        {
            //when user has provided a value
            if (argumentInfo.ValueInfo.HasValue && argumentInfo.ValueInfo.Value != null)
            {
                //parse value
                IParser parser = _parserFactory.CreateInstance(argumentInfo);
                return parser.Parse(argumentInfo);
            }

            //when value not present but method parameter has a default value defined
            if (!argumentInfo.DefaultValue.IsNullValue())
            {
                //use default parameter or property value
                return argumentInfo.DefaultValue;
            }

            //when there no value from input and no default value, return default value of the type
            return argumentInfo.Type.GetDefaultValue();
        }
    }
}