namespace CommandDotNet
{
    public static class Constants
    {
        public const string HelpTemplate = "-h | --help";

        public static readonly ArgumentTemplate HelpArgumentTemplate = new ArgumentTemplate(HelpTemplate);

        public static class TypeDisplayNames
        {
            public const string Flag = null;

            public const string Number = "Number";

            public const string Boolean = "Boolean";

            public const string DoubleNumber = "Double";

            public const string DecimalNumber = "Decimal";

            public const string Character = "Character";

            public const string Text = "Text";
        }
    }
}