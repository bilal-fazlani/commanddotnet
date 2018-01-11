using System;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;

namespace CommandDotNet.Example
{
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
}