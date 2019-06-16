namespace CommandDotNet.Parsing
{
    public class OptionTokenType
    {
        private readonly string _value;
        public bool IsLong { get; }
        public bool IsShort { get; }
        public bool IsClubbed { get; }
        public bool HasValue { get; }
        public int AssignmentIndex { get; }

        public string GetName() => HasValue
            ? _value.Substring(0, AssignmentIndex)
            : _value;

        public string GetAssignedValue() => HasValue
            ? _value.Substring(AssignmentIndex + 1)
            : null;

        /*
         * Value.Substring(0, optionTokenType.AssignmentIndex)
         */

        public OptionTokenType(
            string value,
            bool isLong = false, 
            bool isShort = false, 
            bool isClubbed = false, 
            bool hasValue = false, 
            int assignmentIndex = -1)
        {
            _value = value;
            IsLong = isLong;
            IsShort = isShort;
            IsClubbed = isClubbed;
            HasValue = hasValue;
            AssignmentIndex = assignmentIndex;
        }
    }
}