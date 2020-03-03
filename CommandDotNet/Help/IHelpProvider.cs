namespace CommandDotNet.Help
{
    public interface IHelpProvider
    {
        string GetHelpText(Command command);
    }
}