namespace CommandDotNet.Example
{
    public class SimpleApp
    {
        public IService Service { get; set; }
        
        public int Process()
        {
            return Service.value;
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