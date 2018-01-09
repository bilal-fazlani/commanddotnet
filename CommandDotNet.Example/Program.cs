namespace CommandDotNet.Example
{
    class Program
    {
        static int Main(string[] args)
        {
            AppRunner<MyApplication> appRunner = new AppRunner<MyApplication>();
            return appRunner.Run(args);
        }
    }
}