using System.Collections;
using System.Collections.Generic;

namespace CommandDotNet
{
    public class ErrorText
    {
        public static ErrorText Instance = new ErrorText();

        public virtual string Short_name_must_be_a_single_character() => "Short name must be a single character";

        public virtual string Unknown_argument_type() => "Unknown argument type";

        public virtual string File_not_found(string fullPath) => $"File not found: {fullPath}";

        public virtual string Argument_model_is_invalid(string modelName) => $"'{modelName}' is invalid";

        public virtual IEnumerable<string> DataAnnotation_phrases_to_replace_with_argument_name(string memberName)
        {
            yield return $"The {memberName} field";
            yield return $"The field {memberName}";
            yield return $"The {memberName} property";
            yield return $"The property {memberName}";
        }
    }
}