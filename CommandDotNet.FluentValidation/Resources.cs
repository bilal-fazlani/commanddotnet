namespace CommandDotNet.FluentValidation
{
    public class Resources
    {
        public static Resources A = new();

        public virtual string Error_Argument_model_is_invalid(string modelName) => $"'{modelName}' is invalid";

        public virtual string Error_Could_not_create_instance_of(string name) =>
            $"Could not create instance of {name}. Please ensure it's injected via IoC or has a default constructor.\n" +
            "This exception could also occur if default constructor threw an exception";
    }
}