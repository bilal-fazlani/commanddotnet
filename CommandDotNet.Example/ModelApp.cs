using System;
using System.IO;
using CommandDotNet.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CommandDotNet.Example
{
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
    

    public class Person: IArgumentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Option]
        public string Email { get; set; }
        
        public string Number { get; set; }
    }

    public class Address : IArgumentModel
    {
        public string City { get; set; }
        
        [Option(ShortName = "a")]
        public bool HasAirport { get; set; }
    }
}