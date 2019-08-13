using System;

namespace CommandDotNet.Example.Issues
{
    public class Issue55 : IDisposable
    {
        private readonly string _baseId;
        private readonly string _host;

        [SubCommand]
        public Issue55SubCommand Number { get; set; }
        
        public Issue55([Option(Inherited = true)]string baseId, [Option(Inherited = true)]string host = "localhost:8080")
        {
            _baseId = baseId;
            _host = host;
            
            //some more initial work here
            Console.WriteLine("Initial work...");
        }

        public void getFirmwareVersion()
        {
            Console.WriteLine(_baseId);
            Console.WriteLine(_host);
        }

        public void Dispose()
        {
            Console.WriteLine("Cleanup work ...");
        }
    }
    
    [Command(Name = "number")]
    public class Issue55SubCommand : IDisposable
    {
        public Issue55SubCommand()
        {
            Console.WriteLine("subcommand constructor executed");
        }
        
        public void Print(int number)
        {
            Console.WriteLine(number);
        }
        
        public void Dispose()
        {
            Console.WriteLine("Cleanup work from subcommand...");
        }
    }
}