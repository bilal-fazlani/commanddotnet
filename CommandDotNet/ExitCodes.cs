using System.Threading.Tasks;

namespace CommandDotNet
{
    // begin-snippet: ExitCodes_class
    public static class ExitCodes
    {
        public static int Success => 0;
        public static int Error => 1;
        public static int ValidationError => 2;
        
        public static Task<int> SuccessAsync => Task.FromResult(Success);
        public static Task<int> ErrorAsync => Task.FromResult(Error);
        public static Task<int> ValidationErrorAsync => Task.FromResult(ValidationError);
    }
    // end-snippet
}