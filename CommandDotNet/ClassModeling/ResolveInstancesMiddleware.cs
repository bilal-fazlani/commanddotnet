using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling
{
    internal static class ResolveInstancesMiddleware
    {
        internal static Task<int> ResolveInstances(CommandContext commandContext, ExecutionDelegate next)
        {
            var pipeline = commandContext.InvocationPipeline;
            var instancesToDispose = new List<IDisposable>();

            InvocationStep parentStep = null;
            foreach (var ic in pipeline.All)
            {
                if (parentStep != null 
                    && parentStep.Invocation.MethodInfo.DeclaringType == ic.Invocation.MethodInfo.DeclaringType)
                {
                    // this is true when the interceptor method is in the same class as the command method 
                    ic.Instance = parentStep.Instance;
                    continue;
                }
                ic.Instance = GetInstance(ic, parentStep, commandContext, out bool createdHere);
                parentStep = ic;

                // only dispose instances owned by this middleware, not by a container
                if (createdHere && ic.Instance is IDisposable disposable)
                {
                    instancesToDispose.Add(disposable);
                }
            }

            if (!instancesToDispose.Any())
            {
                return next(commandContext);
            }

            try
            {
                return next(commandContext);
            }
            finally
            {
                // When we support a cli session, we'll need to capture exceptions here
                // and best effort dispose of all instances
                foreach (var disposable in instancesToDispose)
                {
                    disposable.Dispose();
                }
            }
        }

        private static object GetInstance(InvocationStep invocationStep,
            InvocationStep parentStep,
            CommandContext commandContext,
            out bool createdHere)
        {
            var command = invocationStep.Command;
            var commandDef = command.GetCommandDef();
            if (commandDef == null)
            {
                createdHere = false;
                return null;
            }

            var classType = commandDef.CommandHostClassType;
            var resolver = commandContext.AppConfig.DependencyResolver;
            if (resolver != null && resolver.TryResolve(classType, out var instance))
            {
                createdHere = false;
                SetInstanceForParent(parentStep, classType, instance);
                return instance;
            }

            var parameterResolversByType = commandContext.AppConfig.ParameterResolversByType;

            // NOTE: it's possible, that a class could appear more than once in the hierarchy.
            //       because this seems VERY unlikely to be needed, skip this case until
            //       proven it is needed to avoid additional state and complexity to
            //       reuse the instance.  the instance should be reused to map to expectation
            //       for DI that one instance is created per scope.
            var ctor = classType.GetConstructors()
                .Select(c => new { c, p = c.GetParameters() })
                .Where(cp => cp.p.Length == 0 || cp.p.All(p => parameterResolversByType.ContainsKey(p.ParameterType)))
                .OrderByDescending(cp => cp.p.Length)
                .FirstOrDefault();

            if (ctor == null)
            {
                throw new AppRunnerException(
                    $"No viable constructors found for {invocationStep}. " +
                    $"Constructor can only contain parameters of type: {parameterResolversByType.Keys.ToOrderedCsv()}");
            }

            var parameters = ctor.p
                .Select(p => parameterResolversByType[p.ParameterType](commandContext))
                .ToArray();

            instance = ctor.c.Invoke(parameters);
            createdHere = true;
            SetInstanceForParent(parentStep, classType, instance);
            return instance;
        }

        private static void SetInstanceForParent(InvocationStep parentStep, Type classType, object instance)
        {
            if (parentStep != null)
            {
                var parent = parentStep.Instance;
                parent.GetType().GetProperties()
                    .Where(p =>
                        p.CanWrite
                        && p.PropertyType == classType
                        && p.HasAttribute<SubCommandAttribute>()
                        && p.GetValue(parent) == null)
                    .ForEach(p => p.SetValue(parent, instance));
            }
        }
    }
}