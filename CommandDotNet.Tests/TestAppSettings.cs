using System;
using CommandDotNet.Models;
using CommandDotNet.Tests.Utils;

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

        public static AppSettings Clone(this AppSettings appSettings, Action<AppSettings> modify)
        {
            var clone = appSettings.Copy();
            modify(clone);
            return clone;
        }
    }
}