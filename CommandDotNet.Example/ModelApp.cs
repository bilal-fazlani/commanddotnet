using System;
using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CommandDotNet.Example
{
    [Command(Name = "models", Description = "example of using IArgumentModel, including fluent validation.")]
    public class ModelApp
    {
        public int SendNotification(Person person, Address address)
        {
            object obj = new
            {
                person,
                address
            };
            
            string content = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver() 
            });

            Console.WriteLine(content);
            return 5;
        }
    }
    

    [Validator(typeof(PersonValidator))]
    public class Person: IArgumentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Option]
        public string Email { get; set; }
        
        public string Number { get; set; }
        
        [Option]
        public List<string> Alias { get; set; }
    }

    public class Address : IArgumentModel
    {
        public string City { get; set; }
        
        [Option(ShortName = "a")]
        public bool HasAirport { get; set; }
    }

    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}