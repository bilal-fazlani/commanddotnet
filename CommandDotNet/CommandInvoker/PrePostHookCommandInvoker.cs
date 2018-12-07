using System;
using System.Linq;
using CommandDotNet.Extensions;

namespace CommandDotNet.CommandInvoker
{

    [AttributeUsage(AttributeTargets.Method)]
    public class PreHookAttribute : Attribute
    {
        
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class PostHookAttribute : Attribute
    {
        
    }
    
    internal class PrePostHookCommandInvoker : ICommandInvoker
    {
        private readonly ICommandInvoker _commandInvoker;

        public PrePostHookCommandInvoker(ICommandInvoker commandInvoker)
        {
            this._commandInvoker = commandInvoker;
        }

        public object Invoke(CommandInvocation commandInvocation)
        {
            var type = commandInvocation.Instance.GetType();


            try
            {
                ExecuteHooks<PreHookAttribute>(type, commandInvocation);
                return this._commandInvoker.Invoke(commandInvocation);
            }
            finally
            {
                ExecuteHooks<PostHookAttribute>(type, commandInvocation);
            }
        }

        private void ExecuteHooks<T>(Type type, CommandInvocation commandInvocation) where T : Attribute
        {
            var methodInfos = type.GetMethods().Where(m => CustomAttributeProviderExtensions.HasAttribute<T>(m)).ToArray();
            if (methodInfos.Length > 1)
            {
                throw new Exception($"{type.Name} can only have 1 {nameof(T)} but has {methodInfos.Length}");
            }

            var hook = methodInfos.SingleOrDefault();
            if (hook == null)
            {
                return;
            }

            var parameterInfos = hook.GetParameters();
            if (parameterInfos.Any(p => !typeof(IArgumentModel).IsAssignableFrom(p.ParameterType)))
            {
                throw new Exception($"{type.Name} {nameof(T)} only supports paramters of type IArgumentModel");
            }
            var argumentModels = parameterInfos
                .Select(p => commandInvocation.ParamsForCommandMethod.FirstOrDefault(m => p.ParameterType.IsInstanceOfType(m)))
                .ToArray();

            hook.Invoke(commandInvocation.Instance, argumentModels);
        }
    }
}