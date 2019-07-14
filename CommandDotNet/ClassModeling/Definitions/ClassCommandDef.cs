using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class ClassCommandDef : ICommandDef
    {
        private readonly Type _classType;
        private readonly ExecutionConfig _executionConfig;
        private readonly ICommandDef _defaultCommandDef;
        private readonly Lazy<List<ICommandDef>> _subCommands;
        private readonly MethodDef _ctorMethodDef;

        public string Name { get; }

        public ICustomAttributeProvider CustomAttributeProvider => _classType;

        public bool IsExecutable => _defaultCommandDef.IsExecutable;

        public IReadOnlyCollection<IArgumentDef> Arguments => _defaultCommandDef.Arguments;

        public IReadOnlyCollection<ICommandDef> SubCommands => _subCommands.Value;

        public ClassCommandDef(Type classType, ExecutionConfig executionConfig)
        {
            _classType = classType ?? throw new ArgumentNullException(nameof(classType));
            _executionConfig = executionConfig ?? throw new ArgumentNullException(nameof(executionConfig));

            Name = classType.BuildName(executionConfig);

            _defaultCommandDef = GetDefaultMethod();

            _ctorMethodDef = BuildCtorMethod();

            // lazy loading prevents walking the entire hierarchy of sub-commands
            _subCommands = new Lazy<List<ICommandDef>>(GetSubCommands);
        }

        internal static int InvokeMiddleware(CommandContext commandContext, Func<CommandContext, int> next)
        {
            var command = commandContext.ParseResult.Command;
            var commandDef = command.ContextData.Get<ICommandDef>();

            if (commandDef != null)
            {
                return (int)commandDef.Invoke(commandContext, commandDef.Instantiate(commandContext));
            }

            return next(commandContext);
        }

        public object Instantiate(CommandContext commandContext)
        {
            var argumentValues = commandContext.ParseResult.ArgumentValues;

            _ctorMethodDef.Arguments.ForEach(a =>
            {
                if (argumentValues.TryGetValues(a.Name, out var values))
                {
                    a.SetValue(values);
                };
            });

            return _ctorMethodDef.Invoke(null);
        }

        public object Invoke(CommandContext commandContext, object instance)
        {
            return _defaultCommandDef.Invoke(commandContext, instance);
        }

        // TODO: merge ctor options w/ method options (do last)
        // TODO; merge version & help options w/ method options (do very last)

        private MethodDef BuildCtorMethod()
        {
            var firstCtor = _classType.GetConstructors().FirstOrDefault();

            var methodInfo = new MethodDef(firstCtor, _executionConfig);

            if (methodInfo.Arguments.Any(a => a.ArgumentType == ArgumentType.Operand))
            {
                throw new AppRunnerException(
                    $"Constructor arguments can not have [Operand] or [Argument] attribute. Use [Option] attribute. {methodInfo}");
            }

            return methodInfo;
        }

        private ICommandDef GetDefaultMethod()
        {
            var defaultMethod = _classType.GetDeclaredMethods().FirstOrDefault(m => m.HasAttribute<DefaultMethodAttribute>());
            return defaultMethod == null
                ? (ICommandDef)new NullCommandDef(Name)
                : new MethodCommandDef(defaultMethod, Instantiate, _executionConfig);
        }

        private List<ICommandDef> GetSubCommands() => GetLocalSubCommands().Union(GetNestedSubCommands()).ToList();

        private IEnumerable<ICommandDef> GetLocalSubCommands()
        {
            return _classType.GetDeclaredMethods()
                .Where(m => !m.HasAttribute<DefaultMethodAttribute>())
                .Select(m => new MethodCommandDef(m, Instantiate, _executionConfig));
        }

        private IEnumerable<ICommandDef> GetNestedSubCommands()
        {
            IEnumerable<Type> propertySubmodules =
                _classType.GetDeclaredProperties<SubCommandAttribute>()
                    .Select(p => p.PropertyType);

            IEnumerable<Type> inlineClassSubmodules = _classType
                .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(x => x.HasAttribute<SubCommandAttribute>())
                .Where(x => !x.IsCompilerGenerated())
                .Where(x => !typeof(IAsyncStateMachine).IsAssignableFrom(x));

            return propertySubmodules
                .Union(inlineClassSubmodules)
                .Select(t => new ClassCommandDef(t, _executionConfig));
        }
    }
}