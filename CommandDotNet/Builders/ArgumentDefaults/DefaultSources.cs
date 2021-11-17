using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.Builders.ArgumentDefaults
{
    public static class DefaultSources
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public static class EnvVar
        {
            /// <summary>
            /// Returns the value for <see cref="EnvVarAttribute.Key"/> for the <see cref="IArgument"/>
            /// </summary>
            public static IEnumerable<string> GetKeyFromAttribute(IArgument argument)
            {
                var key = argument.GetCustomAttribute<EnvVarAttribute>()?.Key;
                if (key != null)
                {
                    yield return key;
                }
            }

            /// <summary>
            /// The delegate to return the value for the <see cref="IArgument"/>
            /// </summary>
            /// <param name="appRunner"></param>
            /// <param name="envVars">A dictionary containing the environment variables</param>
            /// <param name="getKeysDelegates">
            ///     The delegates that will return the keys for an argument.
            ///     Uses <see cref="GetKeyFromAttribute"/> if none provided.
            /// </param>
            public static Func<IArgument, ArgumentDefault?> GetDefaultValue(
                AppRunner appRunner, IDictionary? envVars = null,
                params GetArgumentKeysDelegate[]? getKeysDelegates)
            {
                IDictionary? ev = envVars;
                // AppConfig should never be null by the time this runs
                IDictionary GetEnvVars()
                {
                    if (appRunner.AppConfig is null)
                    {
                        throw new InvalidConfigurationException(
                            "AppRunner.AppConfig is null. This method has been called before AppRunner.Run was executed.");
                    }
                    return ev ??= appRunner.AppConfig!.Environment.GetEnvironmentVariables();
                }

                return GetValueFunc(Resources.A.ValueSource_EnvVar,
                    key => GetEnvVars().GetValueOrDefault<string>(key),
                    getKeysDelegates ?? new GetArgumentKeysDelegate[]
                    {
                        GetKeyFromAttribute
                    });
            }
        }


        public static class AppSetting
        {
            /// <summary>
            /// Returns the value for <see cref="AppSettingAttribute.Key"/> for the <see cref="IArgument"/>
            /// </summary>
            public static IEnumerable<string> GetKeyFromAttribute(IArgument argument)
            {
                var key = argument.GetCustomAttribute<AppSettingAttribute>()?.Key;
                if (key != null)
                {
                    yield return key;
                }
            }

            /// <summary>
            /// Returns the name of the <see cref="Operand"/> or long and short name of the <see cref="Option"/>.<br/>
            /// Also returns the values prefixed with the defining command for more specificity.<br/>
            /// Example in order: 'delete --force', 'delete -f', '--force', '-f'
            /// </summary>
            public static IEnumerable<string> GetKeysFromConvention(AppSettings appSettings,
                IArgument argument)
            {
                IEnumerable<string> GetOptionKeys(Option option)
                {
                    if (!option.LongName.IsNullOrWhitespace())
                    {
                        yield return $"--{option.LongName}";
                        if (appSettings.Parser.AllowBackslashOptionPrefix)
                        {
                            yield return $"/{option.LongName}";
                        }
                        if (appSettings.Parser.AllowSingleHyphenForLongNames)
                        {
                            yield return $"-{option.LongName}";
                        }
                    }

                    if (option.ShortName.HasValue)
                    {
                        yield return $"-{option.ShortName}";
                        if (appSettings.Parser.AllowBackslashOptionPrefix)
                        {
                            yield return $"/{option.ShortName}";
                        }
                    }
                }

                var keys = argument.SwitchFunc(o => o.Name.ToEnumerable(), GetOptionKeys);
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        yield return $"{argument.Parent!.Name} {key}";
                        yield return key;
                    }
                }
            }

            /// <summary>
            /// The delegate to return the value for the <see cref="IArgument"/>
            /// </summary>
            /// <param name="appSettings">A collection containing the environment variables</param>
            /// <param name="getKeysDelegates">
            /// The delegates that will return the keys for an argument.
            /// Uses <see cref="GetKeyFromAttribute"/> if none provided.
            /// </param>
            public static Func<IArgument, ArgumentDefault?> GetDefaultValue(
                NameValueCollection appSettings,
                params GetArgumentKeysDelegate[]? getKeysDelegates)
            {
                return GetValueFunc(Resources.A.ValueSource_AppSetting,
                    key => appSettings[key],
                    getKeysDelegates ?? new GetArgumentKeysDelegate[]
                    {
                        GetKeyFromAttribute
                    });
            }
        }

        /// <summary>
        /// The delegate to loop through the possible keys for an <see cref="IArgument"/>
        /// and return the first value found using <see cref="getValueForKey"/>
        /// </summary>
        /// <param name="sourceName">Used in logging and CommandLogger to identify the source of a default value</param>
        /// <param name="getValueForKey">The func to return a value for a given key</param>
        /// <param name="getKeysDelegates">The delegates to harvest keys for an <see cref="IArgument"/></param>
        /// <returns></returns>
        public static Func<IArgument, ArgumentDefault?> GetValueFunc(
            string sourceName,
            Func<string, string?> getValueForKey,
            params GetArgumentKeysDelegate[] getKeysDelegates)
        {
            if (sourceName == null)
            {
                throw new ArgumentNullException(nameof(sourceName));
            }

            if (getValueForKey == null)
            {
                throw new ArgumentNullException(nameof(getValueForKey));
            }

            if (getKeysDelegates == null)
            {
                throw new ArgumentNullException(nameof(getKeysDelegates));
            }

            return argument =>
            {
                var value = getKeysDelegates
                    .SelectMany(cb => cb(argument))
                    .Select(key => (key, value:getValueForKey(key)))
                    .Where(v => v.value != null)
                    .Select(v =>
                    {
                        object defaultValue = argument.Arity.AllowsMany()
                            ? v.value!.Split(',')
                            : v.value!;
                        return new ArgumentDefault(sourceName, v.key, defaultValue);
                    })
                    .FirstOrDefault();

                if (value != null)
                {
                    if (Log.IsDebugEnabled())
                    {
                        Log.DebugFormat("default value found in {1} for {0}: {2}={3}",
                            argument,
                            value.Source,
                            value.Key,
                            value.Value.ValueToString(argument)
                        );
                    }
                }
                else
                {
                    Log.DebugFormat("default value not found in `{1}` for {0}", argument, sourceName);
                }

                return value;
            };
        }
    }
}