You can use [FluentValidation](https://github.com/JeremySkinner/FluentValidation) with this library to validate input parameters, provided you model your parameters into classes.

Here's an example,

```c#
    class Program
    {
        static int Main(string[] args)
        {
            AppRunner<ValidationApp> appRunner = new AppRunner<ValidationApp>();
            return appRunner.Run(args);
        }
    }
    public class ValidationApp
    {
        public void ValidateModel(PersonModel person)
        {
            string content = JsonConvert.SerializeObject(person, Formatting.Indented);
            Console.WriteLine(content);
        }
    }

    [Validator(typeof(PersonValidator))]
    public class PersonModel : IArgumentModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Email { get; set; }
    }

    public class PersonValidator : AbstractValidator<PersonModel>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
```

If the validation fails, app exits with return code 2 and prints validation error messages on screen.