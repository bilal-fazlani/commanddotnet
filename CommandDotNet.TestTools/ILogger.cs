namespace CommandDotNet.TestTools
{
    /// <summary>
    /// Used to write test & console output to a test framework.<br/>
    /// i.e. if using XUnit, create an instance to redirect from ITestOutputHelper
    /// </summary>
    public interface ILogger
    {
        void WriteLine(string log);
    }
}