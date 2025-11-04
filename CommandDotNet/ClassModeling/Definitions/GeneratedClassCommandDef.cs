using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using CommandDotNet.Execution;
using CommandDotNet.Extensions;

namespace CommandDotNet.ClassModeling.Definitions;

/// <summary>
/// A command definition for classes that uses generated code instead of runtime reflection.
/// This is used by the source generator to create command definitions at compile time.
/// </summary>
internal class GeneratedClassCommandDef : ICommandDef
{
    private readonly CommandContext _commandContext;
    private readonly ICommandDef? _defaultCommandDef;
    private readonly Lazy<List<ICommandDef>> _subCommands;
    private readonly Func<AppConfig, IMethodDef?> _buildInterceptorMethod;
    private readonly Func<AppConfig, ICommandDef?> _buildDefaultCommand;
    private readonly Func<AppConfig, List<ICommandDef>> _buildLocalCommands;

    public string Name { get; }
    public string SourcePath => CommandHostClassType.FullName!;
    public Type CommandHostClassType { get; }
    public ICustomAttributeProvider CustomAttributes => CommandHostClassType;
    public bool IsExecutable => _defaultCommandDef?.IsExecutable ?? false;
    public bool HasInterceptor => InterceptorMethodDef != null;
    public IReadOnlyCollection<ICommandDef> SubCommands => _subCommands.Value;
    public IMethodDef? InterceptorMethodDef { get; }
    public IMethodDef? InvokeMethodDef => _defaultCommandDef?.InvokeMethodDef;

    /// <summary>
    /// Creates a new GeneratedClassCommandDef using generated builder methods
    /// </summary>
    /// <param name="classType">The command class type</param>
    /// <param name="commandContext">The command context</param>
    /// <param name="buildInterceptorMethod">Generated method to build interceptor method def</param>
    /// <param name="buildDefaultCommand">Generated method to build default command def</param>
    /// <param name="buildLocalCommands">Generated method to build local command defs</param>
    /// <param name="subcommandAttr">Optional subcommand attribute for nested commands</param>
    public GeneratedClassCommandDef(
        Type classType,
        CommandContext commandContext,
        Func<AppConfig, IMethodDef?> buildInterceptorMethod,
        Func<AppConfig, ICommandDef?> buildDefaultCommand,
        Func<AppConfig, List<ICommandDef>> buildLocalCommands,
        SubcommandAttribute? subcommandAttr = null)
    {
        CommandHostClassType = classType ?? throw new ArgumentNullException(nameof(classType));
        _commandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
        _buildInterceptorMethod = buildInterceptorMethod ?? throw new ArgumentNullException(nameof(buildInterceptorMethod));
        _buildDefaultCommand = buildDefaultCommand ?? throw new ArgumentNullException(nameof(buildDefaultCommand));
        _buildLocalCommands = buildLocalCommands ?? throw new ArgumentNullException(nameof(buildLocalCommands));

        Name = classType.BuildName(CommandNodeType.Command, commandContext.AppConfig, subcommandAttr?.RenameAs);

        // Use generated methods instead of reflection
        InterceptorMethodDef = _buildInterceptorMethod(commandContext.AppConfig);
        _defaultCommandDef = _buildDefaultCommand(commandContext.AppConfig);

        // Lazy loading prevents walking the entire hierarchy of sub-commands
        _subCommands = new Lazy<List<ICommandDef>>(() => GetSubCommands(_buildLocalCommands(commandContext.AppConfig)));
    }

    private List<ICommandDef> GetSubCommands(ICollection<ICommandDef> localSubcommands)
    {
        // For now, we still use reflection for nested subcommands
        // This will be enhanced in future iterations to use generated code as well
        var nestedSubcommands = GetNestedSubCommandTypes()
            .Select(t => CreateNestedCommandDef(t.type, t.subcommandAttr));

        return localSubcommands.Union(nestedSubcommands).ToList();
    }

    private IEnumerable<(Type type, SubcommandAttribute subcommandAttr)> GetNestedSubCommandTypes()
    {
        // Use the same logic as ClassCommandDef to find nested subcommands
        IEnumerable<(Type, SubcommandAttribute)> propertySubmodules =
            CommandHostClassType
                .GetDeclaredProperties()
                .Where(x => x.HasAttribute<SubcommandAttribute>())
                .Select(p => (t: p.PropertyType, a: p.GetCustomAttribute<SubcommandAttribute>()))!;

        IEnumerable<(Type, SubcommandAttribute)> inlineClassSubmodules = CommandHostClassType
            .GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
            .Where(t => t.HasAttribute<SubcommandAttribute>())
            .Where(t => !t.IsCompilerGenerated() && !t.IsAssignableTo(typeof(System.Runtime.CompilerServices.IAsyncStateMachine)))
            .Select(t => (t, a: t.GetCustomAttribute<SubcommandAttribute>()))!;

        return propertySubmodules.Union(inlineClassSubmodules);
    }

    private ICommandDef CreateNestedCommandDef(Type type, SubcommandAttribute? attr)
    {
        // Try to use generated command def if available, otherwise fall back to reflection-based ClassCommandDef
        // In the future, the source generator will generate builders for all command classes
        var builderType = type.Assembly.GetType($"{type.FullName}__CommandClassBuilder");
        if (builderType != null)
        {
            var method = builderType.GetMethod("CreateCommandDef", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                return (ICommandDef)method.Invoke(null, new object[] { _commandContext })!;
            }
        }

        // Fallback to reflection-based ClassCommandDef
        var constructor = typeof(ClassCommandDef).GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(Type), typeof(CommandContext), typeof(SubcommandAttribute) },
            null);

        return (ClassCommandDef)constructor!.Invoke(new object?[] { type, _commandContext, attr })!;
    }
}
