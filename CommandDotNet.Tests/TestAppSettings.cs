using System;
using System.Linq;
using System.Reflection;
using CommandDotNet.Models;
using CommandDotNet.Tests.Utils;
using CommandDotNet.TypeDescriptors;

namespace CommandDotNet.Tests
{
    public static class TestAppSettings
    {
        /// <summary>
        /// version is a feature that's enabled for default. 
        /// disabling it for tests reduces the noise in help expectations
        /// and prevents tight coupling to the feature
        /// </summary>
        public static AppSettings TestDefault => new AppSettings{EnableVersionOption = false};
        
        public static AppSettings BasicHelp => new AppSettings{Help = {TextStyle = HelpTextStyle.Basic}, EnableVersionOption = false};
        public static AppSettings DetailedHelp => new AppSettings { Help = { TextStyle = HelpTextStyle.Detailed }, EnableVersionOption = false };

        public static AppSettings Clone(this AppSettings original, Action<AppSettings> modify)
        {
            // var clone = appSettings.Copy();

            var clone = new AppSettings();
            Copy(original, clone);

            clone.Help = new AppHelpSettings();
            Copy(original.Help, clone.Help);

            clone.ArgumentTypeDescriptors = new ArgumentTypeDescriptors(original.ArgumentTypeDescriptors);

            modify(clone);
            return clone;
        }

        private static void Copy<T>(T original, T clone)
        {
            var props = typeof(T)
                .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite);

            foreach (var prop in props)
            {
                prop.SetValue(clone, prop.GetValue(original));
            }
        }
    }
}