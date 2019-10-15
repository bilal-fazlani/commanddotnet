using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Builders;
using CommandDotNet.Extensions;

namespace CommandDotNet.Execution
{
    internal class ResolverService: IDependencyResolver
    {
        internal IDependencyResolver BackingResolver { private get; set; }

        public object Resolve(Type type)
        {
            return BackingResolver?.Resolve(type);
        }

        public bool TryResolve(Type type, out object item)
        {
            item = null;
            return BackingResolver?.TryResolve(type, out item) ?? false;
        }

        internal object ResolveCommandClass(Type classType, CommandContext commandContext)
        {
            if(TryResolve(classType, out var item))
            {
                return item;
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
                    $"No viable constructors found for {classType}. " +
                    $"Constructor can only contain parameters of type: {parameterResolversByType.Keys.ToOrderedCsv()}");
            }

            var parameters = ctor.p
                .Select(p => parameterResolversByType[p.ParameterType](commandContext))
                .ToArray();

            item = ctor.c.Invoke(parameters);

            // only dispose instances created by this middleware, not by a container
            RegisterDisposable(commandContext, item);
            return item;
        }

        private static void RegisterDisposable(CommandContext commandContext, object item)
        {
            if (item is IDisposable disposableItem)
            {
                commandContext.Services.GetOrAdd(() => new Disposables()).Items.Add(disposableItem);
            }
        }

        internal void OnRunCompleted(CommandContext commandContext)
        {
            // When we support a cli session, we'll need to capture exceptions here
            // and best effort dispose of all instances
            commandContext.Services.Get<Disposables>()?.Items.ForEach(i => i.Dispose());
        }

        private class Disposables
        {
            internal readonly List<IDisposable> Items = new List<IDisposable>();
        }
    }
}