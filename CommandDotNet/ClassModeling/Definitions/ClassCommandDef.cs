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
        private readonly CommandContext _commandContext;
        private readonly ICommandDef _defaultCommandDef;
        private readonly Lazy<List<ICommandDef>> _subCommands;

        public string Name { get; }

        public ICustomAttributeProvider CustomAttributeProvider => _classType;

        public bool IsExecutable => _defaultCommandDef.IsExecutable;

        public IReadOnlyCollection<IArgumentDef> Arguments { get; }

        public IReadOnlyCollection<ICommandDef> SubCommands => _subCommands.Value;

        public IMethodDef InstantiateMethodDef { get; }
        
        public IMethodDef InvokeMethodDef => _defaultCommandDef.InvokeMethodDef;

        public Command Command { get; set; }

        public static Command CreateRootCommand(Type classType, CommandContext commandContext)
        {
            return new ClassCommandDef(classType, commandContext)
                .ToCommand(null, commandContext)
                .Command;
        }

        private ClassCommandDef(Type classType, CommandContext commandContext)
        {
            _classType = classType ?? throw new ArgumentNullException(nameof(classType));
            _commandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));

            Name = classType.BuildName(commandContext.ExecutionConfig);

            _defaultCommandDef = GetDefaultMethod();

            InstantiateMethodDef = BuildCtorMethod();

            Arguments = _defaultCommandDef.Arguments.Union(InstantiateMethodDef.ArgumentDefs).ToArray();

            // lazy loading prevents walking the entire hierarchy of sub-commands
            _subCommands = new Lazy<List<ICommandDef>>(GetSubCommands);
        }

        private MethodDef BuildCtorMethod()
        {
            var firstCtor = _classType.GetConstructors().FirstOrDefault();

            var methodInfo = new MethodDef(firstCtor, _commandContext.ExecutionConfig);

            if (methodInfo.ArgumentDefs.Any(a => a.ArgumentType == ArgumentType.Operand))
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
                : new MethodCommandDef(defaultMethod, InstantiateMethodDef, _commandContext.ExecutionConfig);
        }

        private List<ICommandDef> GetSubCommands() => GetLocalSubCommands().Union(GetNestedSubCommands()).ToList();

        private IEnumerable<ICommandDef> GetLocalSubCommands()
        {
            return _classType.GetDeclaredMethods()
                .Where(m => !m.HasAttribute<DefaultMethodAttribute>())
                .Select(m => new MethodCommandDef(m, InstantiateMethodDef, _commandContext.ExecutionConfig));
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
                .Select(t => new ClassCommandDef(t, _commandContext));
        }
    }
}