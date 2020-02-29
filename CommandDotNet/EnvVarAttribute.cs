using System;

namespace CommandDotNet
{
    /// <summary>
    /// Sets the default value of this argument from the environment variable with the given key.<br/>
    /// NOTE: register with `appRunner.UseDefaultsFromEnvVar`
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class EnvVarAttribute : Attribute
    {
        /// <summary>The key of the environment variable to use for a default value</summary>
        public string Key { get; }

        /// <summary></summary>
        /// <param name="key">The key of the environment variable to use for a default value</param>
        public EnvVarAttribute(string key) => Key = key;
    }
}