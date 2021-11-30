namespace CommandDotNet.TestTools.Prompts
{
    public class UnexpectedPromptFailureException : AssertFailedException
    {
        public UnexpectedPromptFailureException(string promptText)
            : base($"unexpected prompt: {promptText}")
        {
        }
    }
}