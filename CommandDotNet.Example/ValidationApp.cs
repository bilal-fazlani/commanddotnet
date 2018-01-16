using System;
using CommandDotNet.Attributes;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;

namespace CommandDotNet.Example
{
    public class ValidationApp
    {
        public void ValidateModel(PersonModel person, string pinCode = "400613")
        {
            string content = JsonConvert.SerializeObject(new {person, pinCode}, Formatting.Indented);
            Console.WriteLine(content);
        }
    }

    [Validator(typeof(PersonValidator))]
    public class PersonModel : IArgumentModel
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        [Option]
        public int Age { get; set; }
        
        public string Email { get; set; }
        
        public City City { get; set; }

        public string Phone { get; set; } = "9833";
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

    public enum City
    {
        Mumbai = 10,
        Delhi = 20
    }
}