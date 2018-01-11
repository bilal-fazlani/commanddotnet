using System.IO;
using Newtonsoft.Json;

namespace CommandDotNet.Tests
{
    public static class Extensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }

        public static void WriteToFile(this string value, string path)
        {
            File.WriteAllText(path, value);
        }
    }
}