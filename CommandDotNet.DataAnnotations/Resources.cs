using System.Collections.Generic;

namespace CommandDotNet.DataAnnotations
{
    public class Resources
    {
        public static Resources A = new Resources();

        public virtual IEnumerable<string> Error_DataAnnotation_phrases_to_replace_with_argument_name(string memberName)
        {
            yield return $"The {memberName} field";
            yield return $"The field {memberName}";
            yield return $"The {memberName} property";
            yield return $"The property {memberName}";
        }
    }
}