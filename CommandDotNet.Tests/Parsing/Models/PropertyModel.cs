using System.Collections.Generic;

namespace CommandDotNet.Tests.Parsing.Models
{
    public class PropertyModel
    {
        public int Int { get; set; }
        public int? NullableInt { get; set; }
        public List<int> ListInt { get; set; }
        
        public string String { get; set; }
        public List<string> ListString { get; set; }

        
        public double Double { get; set; }
        public long Long { get; set; }
        public bool Bool { get; set; }
        public char Char { get; set; }
        public decimal Decimal { get; set; }

        public Time Time { get; set; }
        public Time? NullableTime { get; set; }
        public List<Time> ListTime { get; set; }

        public Person Person { get; set; }
        public List<Person> ListPerson { get; set; }
    }
}