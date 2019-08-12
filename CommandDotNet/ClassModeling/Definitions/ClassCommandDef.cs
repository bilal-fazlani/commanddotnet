using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class ClassCommandDef : ICommandDef
    {
        private readonly CommandContext _commandContext;
        private readonly ICommandDef _defaultCommandDef;
        private readonly Lazy<List<ICommandDef>> _subCommands;

        public string Name { get; }

        public Type CommandHostClassType { get; }

        public ICustomAttributeProvider CustomAttributeProvider => CommandHostClassType;

        public bool IsExecutable => _defaultCommandDef.IsExecutable;

        public IReadOnlyCollection<IArgumentDef> Arguments { get; }

        public IReadOnlyCollection<ICommandDef> SubCommands => _subCommands.Value;

        public IMethodDef MiddlewareMethodDef { get; }

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
            CommandHostClassType = classType ?? throw new ArgumentNullException(nameof(classType));
            _commandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));

            Name = classType.BuildName(commandContext.AppConfig);

            var (middlewareMethod, defaultCommand, localCommands) = ParseMethods(commandContext.AppConfig);

            MiddlewareMethodDef = middlewareMethod;
            _defaultCommandDef = defaultCommand;

            Arguments = _defaultCommandDef.Arguments
                .Union(MiddlewareMethodDef.ArgumentDefs)
                .ToArray();

            // lazy loading prevents walking the entire hierarchy of sub-commands
            _subCommands = new Lazy<List<ICommandDef>>(() => GetSubCommands(localCommands));
        }

        private (IMethodDef middlewareMethod, ICommandDef defaultCommand, List<ICommandDef> localCommands) ParseMethods(AppConfig appConfig)
        {
            MethodInfo middlewareMethodInfo = null;
            MethodInfo defaultCommandMethodInfo = null;
            List<MethodInfo> localCommandMethodInfos = new List<MethodInfo>();

            foreach (var method in CommandHostClassType.GetDeclaredMethods())
            {
                if (MethodDef.IsMiddlewareMethod(method))
                {
                    if (middlewareMethodInfo != null)
                    {
                        throw new InvalidConfigurationException($"`{CommandHostClassType}` defines more than one middleware method with a parameter of type {MethodDef.MiddlewareNextParameterType}.  There can be only one.");
                    }
                    if (method.HasAttribute<DefaultMethodAttribute>())
                    {
                        throw new InvalidConfigurationException($"`{CommandHostClassType}.{method.Name}` default method cannot contain parameter of type {MethodDef.MiddlewareNextParameterType}.");
                    }

                    var emDelegate = new ExecutionMiddleware((context, next) => Task.FromResult(1)).Method;
                    if (method.ReturnType != emDelegate.ReturnType)
                    {
                        throw new InvalidConfigurationException($"`{CommandHostClassType}.{method.Name}` must return type of {emDelegate.ReturnType}, matching {typeof(ExecutionMiddleware)}.");
                    }

                    middlewareMethodInfo = method;
                }
                else if (method.HasAttribute<DefaultMethodAttribute>())
                {
                    if (defaultCommandMethodInfo != null)
                    {
                        throw new InvalidConfigurationException($"`{CommandHostClassType}` defines more than one method with {nameof(DefaultMethodAttribute)}.  There can be only one.");
                    }
                    defaultCommandMethodInfo = method;
                }
                else
                {
                    localCommandMethodInfos.Add(method);
                }
            }

            var middlewareMethod = middlewareMethodInfo == null 
                ? NullMethodDef.Instance 
                : new MethodDef(middlewareMethodInfo, appConfig);

            var defaultCommand = defaultCommandMethodInfo == null
                ? (ICommandDef)new NullCommandDef(Name)
                : new MethodCommandDef(defaultCommandMethodInfo, CommandHostClassType, middlewareMethod, appConfig);
            
            return (
                middlewareMethod,
                defaultCommand,
                localCommandMethodInfos
                    .Select(m => new MethodCommandDef(m, CommandHostClassType, middlewareMethod, appConfig))
                    .Cast<ICommandDef>()
                    .ToList());

        }

        private List<ICommandDef> GetSubCommands(IEnumerable<ICommandDef> localSubCommands) => localSubCommands.Union(GetNestedSubCommands()).ToList();

        private IEnumerable<ICommandDef> GetNestedSubCommands()
        {
            IEnumerable<Type> propertySubmodules =
                CommandHostClassType.GetDeclaredProperties<SubCommandAttribute>()
                    .Select(p => p.PropertyType);

            IEnumerable<Type> inlineClassSubmodules = CommandHostClassType
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