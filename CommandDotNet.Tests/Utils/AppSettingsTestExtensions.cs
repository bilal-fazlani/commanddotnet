using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Tests.Utils
{
    public static class AppSettingsTestExtensions
    {
        public static AppSettings Add<T>(this AppSettings appSettings) 
            where T : IArgumentTypeDescriptor, new()
        {
            appSettings.ArgumentTypeDescriptors.Add(new T());
            return appSettings;
        }
    }
}