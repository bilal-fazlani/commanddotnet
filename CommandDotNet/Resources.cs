namespace CommandDotNet
{
    public partial class Resources
    {
        public static Resources A = new Resources();

        public virtual string Error_File_not_found(string fullPath) => $"File not found: {fullPath}";
    }
}
