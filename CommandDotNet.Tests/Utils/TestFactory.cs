using System.Collections.Generic;
using System.Reflection;
using CommandDotNet.Models;

namespace CommandDotNet.Tests.Utils
{
    public static class TestFactory
    {
        private static readonly AppSettings DefaultAppSettings = new AppSettings();

        public static IEnumerable<ArgumentInfo> GetArgumentsFromMethod<TMethodHost>(string methodName, AppSettings appSettings = null)
        {
            return typeof(TMethodHost).GetMethod(methodName).GetArgumentsFromMethod(appSettings);
        }
        
        public static IEnumerable<ArgumentInfo> GetArgumentsFromMethod(this MethodInfo methodInfo, AppSettings appSettings = null)
        {
            return new ArgumentInfoCreator(appSettings ?? DefaultAppSettings)
                .GetArgumentsFromMethod(methodInfo, ArgumentMode.Option);
        }
        
        public static IEnumerable<ArgumentInfo> GetArgumentsFromModel<TModel>(AppSettings appSettings = null)
        {
            return new ArgumentInfoCreator(appSettings ?? DefaultAppSettings)
                .GetArgumentsFromModel(typeof(TModel), ArgumentMode.Option);
        }
    }
}