using Newtonsoft.Json;

namespace CommandDotNet.Tests
{
    public static class Extensions
    {
        public static string ToJson(this object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented);
        }
    }
}