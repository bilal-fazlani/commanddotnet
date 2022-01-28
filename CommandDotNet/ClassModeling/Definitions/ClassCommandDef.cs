using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;
using CommandDotNet.Logging;

namespace CommandDotNet.ClassModeling.Definitions
{
    internal class ClassCommandDef : ICommandDef
    {
        private static ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly CommandContext _commandContext;
        private readonly ICommandDef? _defaultCommandDef;
        private readonly Lazy<List<ICommandDef>> _subCommands;

        public string Name { get; }
        public string SourcePath => CommandHostClassType.FullName!;

        public Type CommandHostClassType { get; }

        public ICustomAttributeProvider CustomAttributes => CommandHostClassType;

        public bool IsExecutable => _defaultCommandDef?.IsExecutable ?? false;

        public bool HasInterceptor => InterceptorMethodDef != null;

        public IReadOnlyCollection<ICommandDef> SubCommands => _subCommands.Value;

        public IMethodDef? InterceptorMethodDef { get; }

        public IMethodDef? InvokeMethodDef => _defaultCommandDef?.InvokeMethodDef;

        public static Command CreateRootCommand(Type rootAppType, CommandContext commandContext)
        {
            Log.Debug("begin {0}: rootAppType={1}", nameof(CreateRootCommand), rootAppType);
            var rootCommand = new ClassCommandDef(rootAppType, commandContext)
                .ToCommand(null, commandContext)
                .Command;
            Log.Debug("end {0}");
            return rootCommand;
        }

        public static IEnumerable<(Type type, SubcommandAttribute? subcommandAttr)> GetAllCommandClassTypes(
            Type rootAppType, SubcommandAttribute? subcommandAttr = null)
        {
            var childTypes = GetNestedSubCommandTypes(rootAppType)
                .SelectMany(x => GetAllCommandClassTypes(x.type, x.subcommandAttr));
            return (rootAppType, subcommandAttribute: subcommandAttr)
                .ToEnumerable()
                .Union(childTypes);
        }

        private ClassCommandDef(Type classType, CommandContext commandContext, SubcommandAttribute? subcommandAttr = null)
        {
            CommandHostClassType = classType ?? throw new ArgumentNullException(nameof(classType));
            _commandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));

            Name = classType.BuildName(CommandNodeType.Command, commandContext.AppConfig, subcommandAttr?.RenameAs);

            var (interceptorMethod, defaultCommand, localCommands) = ParseMethods(commandContext.AppConfig);

            InterceptorMethodDef = interceptorMethod;
            _defaultCommandDef = defaultCommand;

            // lazy loading prevents walking the entire hierarchy of sub-commands
            _subCommands = new Lazy<List<ICommandDef>>(() => GetSubCommands(localCommands));
        }

        private (IMethodDef? interceptorMethod, ICommandDef? defaultCommand, List<ICommandDef> localCommands) 
            ParseMethods(AppConfig appConfig)
        {
            MethodInfo? interceptorMethodInfo = null;
            MethodInfo? defaultCommandMethodInfo = null;
            List<MethodInfo> localCommandMethodInfos = new();

            foreach (var method in CommandHostClassType.GetCommandMethods(appConfig.AppSettings.Commands.InheritCommandsFromBaseClasses))
            {
                if (MethodDef.IsInterceptorMethod(method))
                {
                    if (interceptorMethodInfo != null)
                    {
                        throw new InvalidConfigurationException($"`{CommandHostClassType}` defines more than one middleware method with a parameter of type " +
                                                                $"{MethodDef.MiddlewareNextParameterType} or {MethodDef.InterceptorNextParameterType}. " +
                                                                "There can be only one.");
                    }
                    if (method.HasAttribute<DefaultCommandAttribute>())
                    {
                        throw new InvalidConfigurationException($"`{CommandHostClassType}.{method.Name}` default method cannot contain parameter of type " +
                                                                $"{MethodDef.MiddlewareNextParameterType} or {MethodDef.InterceptorNextParameterType}.");
                    }

                    var emDelegate = new ExecutionMiddleware((_, _) => ExitCodes.Error).Method;
                    if (method.ReturnType != emDelegate.ReturnType)
                    {
                        throw new InvalidConfigurationException($"`{CommandHostClassType}.{method.Name}` must return type of {emDelegate.ReturnType}.");
                    }

                    interceptorMethodInfo = method;
                }
                else if (method.HasAttribute<DefaultCommandAttribute>())
                {
                    if (defaultCommandMethodInfo != null)
                    {
                        throw new InvalidConfigurationException($"`{CommandHostClassType}` defines more than one method with {nameof(DefaultCommandAttribute)}.  There can be only one.");
                    }
                    defaultCommandMethodInfo = method;
                }
                else
                {
                    localCommandMethodInfos.Add(method);
                }
            }

            var interceptorMethod = interceptorMethodInfo == null 
                ? null
                : new MethodDef(interceptorMethodInfo, appConfig, true);

            var defaultCommand = defaultCommandMethodInfo == null
                ? null
                : new MethodCommandDef(defaultCommandMethodInfo, CommandHostClassType, appConfig);
            
            return (
                interceptorMethod,
                defaultCommand,
                localCommandMethodInfos
                    .Select(m => new MethodCommandDef(m, CommandHostClassType, appConfig))
                    .Cast<ICommandDef>()
                    .ToList());

        }

        private List<ICommandDef> GetSubCommands(ICollection<ICommandDef> localSubcommands)
        {
            var allSubcommands = localSubcommands.Union(GetNestedSubCommands()).ToList();
            Log.Debug("begin {0}: type={1} local={2} nested={3}", nameof(GetSubCommands), this.CommandHostClassType,
                localSubcommands.Count, 
                allSubcommands.Count-localSubcommands.Count);
            return allSubcommands;
        }

        private IEnumerable<ICommandDef> GetNestedSubCommands()
        {
            return GetNestedSubCommandTypes(CommandHostClassType)
                .Select(t => new ClassCommandDef(t.type, _commandContext, t.subcommandAttr));
        }

        private static IEnumerable<(Type type, SubcommandAttribute subcommandAttr)> GetNestedSubCommandTypes(Type classType)
        {
            IEnumerable<(Type, SubcommandAttribute)> propertySubmodules =
                classType
                    .GetDeclaredProperties()
                    .Where(x => x.HasAttribute<SubcommandAttribute>())
                    .Select(p => (t: p.PropertyType, a: p.GetCustomAttribute<SubcommandAttribute>()))!;

            IEnumerable<(Type, SubcommandAttribute)> inlineClassSubmodules = classType
                .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(t => t.HasAttribute<SubcommandAttribute>())
                .Where(t => !t.IsCompilerGenerated() && !t.IsAssignableTo(typeof(IAsyncStateMachine)))
                .Select(t => (t, a: t.GetCustomAttribute<SubcommandAttribute>()))!;

            return propertySubmodules
                .Union(inlineClassSubmodules);
        }
    }
}