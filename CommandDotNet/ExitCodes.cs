using System.Threading.Tasks;

namespace CommandDotNet
{
    // begin-snippet: ExitCodes_class
    public static class ExitCodes
    {
        public static Task<int> Success => Task.FromResult(0);
        public static Task<int> Error => Task.FromResult(1);
        public static Task<int> ValidationError => Task.FromResult(2);
    }
    // end-snippet
}