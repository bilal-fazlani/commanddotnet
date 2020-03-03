using System;

namespace CommandDotNet
{
    /// <summary>
    /// Sets the default value of this argument from the AppSettings with the given key.<br/>
    /// NOTE: register with `appRunner.UseDefaultsFromAppSetting`
    /// </summary>
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Parameter)]
    public class AppSettingAttribute : Attribute
    {
        /// <summary>The key of the AppSettings to use for a default value</summary>
        public string Key { get; }

        /// <summary></summary>
        /// <param name="key">The key of the AppSettings to use for a default value</param>
        public AppSettingAttribute(string key) => Key = key;
    }
}