using System;
using CommandDotNet.Attributes;

namespace CommandDotNet.Example
{
    public class ServiceApp
    {
        public IService Service { get; set; }
        
        [DefaultMethod]
        public void Process()
        {
            Console.WriteLine($"Service value is {Service.value}");
        }
    }

    public interface IService
    {
        int value { get; set; }
    }

    public class Service : IService
    {
        public int value { get; set; } = 4;
    }
}