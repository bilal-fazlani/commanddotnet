namespace CommandDotNet.DataAnnotations
{
    public class Resources
    {
        public static Resources A = new();

        public virtual string Error_DataAnnotation_phrases_to_replace_with_argument_name() => 
            "The {0} field|The field {0}|The {0} property|The property {0}";
    }
}