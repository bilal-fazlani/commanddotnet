using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CommandDotNet.SourceGen;

/// <summary>
/// Generates static class builders to replace runtime reflection for CommandDotNet command classes
/// </summary>
[Generator]
public partial class CommandClassGenerator : IIncrementalGenerator
{
    private const string CommandAttributeFullName = "CommandDotNet.CommandAttribute";
    private const string SubcommandAttributeFullName = "CommandDotNet.SubcommandAttribute";
    private const string DefaultCommandAttributeFullName = "CommandDotNet.DefaultCommandAttribute";

    /// <summary>
    /// Gets the builder class name for a command class, handling nested classes
    /// </summary>
    private static string GetBuilderClassName(CommandClassInfo classInfo)
    {
        return classInfo.FullyQualifiedName
            .Replace("global::", "")
            .Replace(classInfo.Namespace + ".", "")  // Remove namespace part
            .Replace(".", "_")  // Replace dots with underscores
            .Replace("+", "_"); // Replace nested class separator
    }

    /// <summary>
    /// Checks if a type and all its containing types are public
    /// </summary>
    private static bool IsFullyPublic(INamedTypeSymbol typeSymbol)
    {
        var current = typeSymbol;
        while (current != null)
        {
            if (current.DeclaredAccessibility != Accessibility.Public)
                return false;
            current = current.ContainingType;
        }
        return true;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all classes that are potential command classes
        var commandClasses = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCommandClass(node),
                transform: static (ctx, _) => GetCommandClassInfo(ctx))
            .Where(static m => m is not null);

        // Collect all command classes to generate module initializer
        var allCommandClasses = commandClasses.Collect();

        // Generate individual class builders
        context.RegisterSourceOutput(commandClasses, 
            static (spc, commandClass) => GenerateClassBuilder(spc, commandClass!));

        // Generate module initializer that registers all builders
        context.RegisterSourceOutput(allCommandClasses,
            static (spc, commandClasses) => GenerateModuleInitializer(spc, commandClasses));
    }

    private static bool IsCommandClass(SyntaxNode node)
    {
        // A command class is a class that:
        // 1. Has methods (potential command methods)
        // 2. Has properties with SubcommandAttribute
        // 3. Has nested types with SubcommandAttribute
        return node is ClassDeclarationSyntax classDecl 
            && classDecl.Members.Any(m => m is MethodDeclarationSyntax || m is PropertyDeclarationSyntax);
    }

    private static CommandClassInfo? GetCommandClassInfo(GeneratorSyntaxContext context)
    {
        try
        {
            var classDecl = (ClassDeclarationSyntax)context.Node;
            var model = context.SemanticModel;
            var classSymbol = model.GetDeclaredSymbol(classDecl);

            if (classSymbol == null)
                return null;

            // Only generate for public classes
            // For nested classes, ALL containing types must also be public
            if (!IsFullyPublic(classSymbol))
                return null;

            // Check if this class has command methods
            var hasCommandMethods = classSymbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Any(m => m.DeclaredAccessibility == Accessibility.Public && !m.IsStatic && m.MethodKind == MethodKind.Ordinary);

            if (!hasCommandMethods)
                return null;

            var commandMethods = GetCommandMethods(classSymbol);
            var subcommandProperties = GetSubcommandProperties(classSymbol);
            var nestedSubcommands = GetNestedSubcommands(classSymbol);

            return new CommandClassInfo(
                classSymbol.ContainingNamespace.ToDisplayString(),
                classSymbol.Name,
                classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                commandMethods,
                subcommandProperties,
                nestedSubcommands);
        }
        catch
        {
            // If we can't analyze this class, skip it
            return null;
        }
    }

    private static List<MethodInfo> GetCommandMethods(INamedTypeSymbol classSymbol)
    {
        var methods = new List<MethodInfo>();

        foreach (var member in classSymbol.GetMembers())
        {
            if (member is IMethodSymbol method 
                && method.DeclaredAccessibility == Accessibility.Public 
                && !method.IsStatic
                && method.MethodKind == MethodKind.Ordinary)
            {
                var isInterceptor = method.Parameters.Any(p => 
                    p.Type.ToDisplayString() == "CommandDotNet.Execution.ExecutionDelegate" ||
                    p.Type.ToDisplayString() == "CommandDotNet.Execution.InterceptorExecutionDelegate");

                var isDefaultCommand = method.GetAttributes()
                    .Any(a => a.AttributeClass?.ToDisplayString() == DefaultCommandAttributeFullName);

                methods.Add(new MethodInfo(
                    method.Name,
                    isInterceptor,
                    isDefaultCommand,
                    GetParameters(method)));
            }
        }

        return methods;
    }

    private static List<ParameterInfo> GetParameters(IMethodSymbol method)
    {
        return method.Parameters
            .Select(p => new ParameterInfo(
                p.Name,
                p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                p.HasExplicitDefaultValue,
                p.ExplicitDefaultValue?.ToString() ?? "null"))
            .ToList();
    }

    private static List<PropertyInfo> GetSubcommandProperties(INamedTypeSymbol classSymbol)
    {
        var properties = new List<PropertyInfo>();

        foreach (var member in classSymbol.GetMembers())
        {
            if (member is IPropertySymbol property
                && property.DeclaredAccessibility == Accessibility.Public)
            {
                var hasSubcommandAttr = property.GetAttributes()
                    .Any(a => a.AttributeClass?.ToDisplayString() == SubcommandAttributeFullName);

                if (hasSubcommandAttr)
                {
                    properties.Add(new PropertyInfo(
                        property.Name,
                        property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
                }
            }
        }

        return properties;
    }

    private static List<string> GetNestedSubcommands(INamedTypeSymbol classSymbol)
    {
        var nested = new List<string>();

        foreach (var member in classSymbol.GetMembers())
        {
            if (member is INamedTypeSymbol nestedType)
            {
                var hasSubcommandAttr = nestedType.GetAttributes()
                    .Any(a => a.AttributeClass?.ToDisplayString() == SubcommandAttributeFullName);

                if (hasSubcommandAttr)
                {
                    nested.Add(nestedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
                }
            }
        }

        return nested;
    }

    private static void GenerateClassBuilder(SourceProductionContext context, CommandClassInfo classInfo)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using global::System;");
        sb.AppendLine("using global::System.Collections.Generic;");
        sb.AppendLine("using global::CommandDotNet.ClassModeling.Definitions;");
        sb.AppendLine("using global::CommandDotNet.Execution;");
            
        sb.AppendLine();
        sb.AppendLine($"namespace {classInfo.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"internal static class {GetBuilderClassName(classInfo)}__CommandClassBuilder");
        sb.AppendLine("{");

        // Generate method to create command definition
        sb.AppendLine($"    public static ICommandDef CreateCommandDef(CommandContext commandContext)");
        sb.AppendLine("    {");
        sb.AppendLine($"        return new GeneratedClassCommandDef(");
        sb.AppendLine($"            typeof({classInfo.FullyQualifiedName}),");
        sb.AppendLine($"            commandContext,");
        sb.AppendLine($"            BuildInterceptorMethod,");
        sb.AppendLine($"            BuildDefaultCommand,");
        sb.AppendLine($"            BuildLocalCommands);");
        sb.AppendLine("    }");
        sb.AppendLine();

        // Generate interceptor method builder
        GenerateInterceptorMethodBuilder(sb, classInfo);

        // Generate default command builder
        GenerateDefaultCommandBuilder(sb, classInfo);

        // Generate local commands builder
        GenerateLocalCommandsBuilder(sb, classInfo);

        sb.AppendLine("}");

        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
        // Use fully qualified name to ensure uniqueness (handles nested classes)
        var hintName = $"{classInfo.FullyQualifiedName}__CommandClassBuilder.g.cs"
            .Replace("global::", "")
            .Replace("<", "_")
            .Replace(">", "_")
            .Replace(".", "_")
            .Replace("+", "_");  // Nested class separator
        context.AddSource(hintName, sourceText);
    }

    private static void GenerateInterceptorMethodBuilder(StringBuilder sb, CommandClassInfo classInfo)
    {
        var interceptor = classInfo.Methods.FirstOrDefault(m => m.IsInterceptor);

        sb.AppendLine("    private static IMethodDef? BuildInterceptorMethod(AppConfig appConfig)");
        sb.AppendLine("    {");
        
        if (interceptor != null)
        {
            sb.AppendLine($"        var method = typeof({classInfo.FullyQualifiedName}).GetMethod(\"{interceptor.Name}\");");
            sb.AppendLine("        return new global::CommandDotNet.ClassModeling.Definitions.MethodDef(method!, appConfig, true);");
        }
        else
        {
            sb.AppendLine("        return null;");
        }

        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void GenerateDefaultCommandBuilder(StringBuilder sb, CommandClassInfo classInfo)
    {
        var defaultCmd = classInfo.Methods.FirstOrDefault(m => m.IsDefaultCommand);

        sb.AppendLine("    private static ICommandDef? BuildDefaultCommand(AppConfig appConfig)");
        sb.AppendLine("    {");

        if (defaultCmd != null)
        {
            sb.AppendLine($"        var method = typeof({classInfo.FullyQualifiedName}).GetMethod(\"{defaultCmd.Name}\");");
            sb.AppendLine($"        return new global::CommandDotNet.ClassModeling.Definitions.MethodCommandDef(");
            sb.AppendLine($"            method!, typeof({classInfo.FullyQualifiedName}), appConfig);");
        }
        else
        {
            sb.AppendLine("        return null;");
        }

        sb.AppendLine("    }");
        sb.AppendLine();
    }

    private static void GenerateLocalCommandsBuilder(StringBuilder sb, CommandClassInfo classInfo)
    {
        var localCommands = classInfo.Methods
            .Where(m => !m.IsInterceptor && !m.IsDefaultCommand)
            .ToList();

        sb.AppendLine("    private static List<ICommandDef> BuildLocalCommands(AppConfig appConfig)");
        sb.AppendLine("    {");
        sb.AppendLine("        var commands = new List<ICommandDef>();");
        sb.AppendLine();

        foreach (var method in localCommands)
        {
            sb.AppendLine($"        {{");
            sb.AppendLine($"            var method = typeof({classInfo.FullyQualifiedName}).GetMethod(\"{method.Name}\");");
            sb.AppendLine($"            commands.Add(new global::CommandDotNet.ClassModeling.Definitions.MethodCommandDef(");
            sb.AppendLine($"                method!, typeof({classInfo.FullyQualifiedName}), appConfig));");
            sb.AppendLine($"        }}");
        }

        sb.AppendLine();
        sb.AppendLine("        return commands;");
        sb.AppendLine("    }");
    }

    private static void GenerateModuleInitializer(
        SourceProductionContext context,
        System.Collections.Immutable.ImmutableArray<CommandClassInfo?> commandClasses)
    {
        var validClasses = commandClasses.Where(c => c != null).ToList();
        
        if (validClasses.Count == 0)
            return;

        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using System.Runtime.CompilerServices;");
        sb.AppendLine("using CommandDotNet.ClassModeling.Definitions;");
        sb.AppendLine();
        sb.AppendLine("namespace CommandDotNet.Generated;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Module initializer that registers all source-generated command builders.");
        sb.AppendLine("/// This runs automatically at app startup, eliminating runtime reflection.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("internal static class GeneratedBuildersInitializer");
        sb.AppendLine("{");
        sb.AppendLine("    [ModuleInitializer]");
        sb.AppendLine("    internal static void Initialize()");
        sb.AppendLine("    {");
        
        // Register each command class builder
        foreach (var classInfo in validClasses)
        {
            if (classInfo == null) continue;
            
            sb.AppendLine($"        // Register builder for {classInfo.ClassName}");
            sb.AppendLine($"        CommandDefRegistry.Register<{classInfo.FullyQualifiedName}>(");
            sb.AppendLine($"            {classInfo.Namespace}.{GetBuilderClassName(classInfo)}__CommandClassBuilder.CreateCommandDef);");
        }
        
        sb.AppendLine("    }");
        sb.AppendLine("}");

        var sourceText = SourceText.From(sb.ToString(), Encoding.UTF8);
        context.AddSource("GeneratedBuildersInitializer.g.cs", sourceText);
    }
}

internal class CommandClassInfo
{
    public string Namespace { get; }
    public string ClassName { get; }
    public string FullyQualifiedName { get; }
    public List<MethodInfo> Methods { get; }
    public List<PropertyInfo> SubcommandProperties { get; }
    public List<string> NestedSubcommands { get; }

    public CommandClassInfo(string @namespace, string className, string fullyQualifiedName,
        List<MethodInfo> methods, List<PropertyInfo> subcommandProperties, List<string> nestedSubcommands)
    {
        Namespace = @namespace;
        ClassName = className;
        FullyQualifiedName = fullyQualifiedName;
        Methods = methods;
        SubcommandProperties = subcommandProperties;
        NestedSubcommands = nestedSubcommands;
    }
}

internal class MethodInfo
{
    public string Name { get; }
    public bool IsInterceptor { get; }
    public bool IsDefaultCommand { get; }
    public List<ParameterInfo> Parameters { get; }

    public MethodInfo(string name, bool isInterceptor, bool isDefaultCommand, List<ParameterInfo> parameters)
    {
        Name = name;
        IsInterceptor = isInterceptor;
        IsDefaultCommand = isDefaultCommand;
        Parameters = parameters;
    }
}

internal class ParameterInfo
{
    public string Name { get; }
    public string TypeFullName { get; }
    public bool HasDefaultValue { get; }
    public string? DefaultValue { get; }

    public ParameterInfo(string name, string typeFullName, bool hasDefaultValue, string? defaultValue)
    {
        Name = name;
        TypeFullName = typeFullName;
        HasDefaultValue = hasDefaultValue;
        DefaultValue = defaultValue;
    }
}

internal class PropertyInfo
{
    public string Name { get; }
    public string TypeFullName { get; }

    public PropertyInfo(string name, string typeFullName)
    {
        Name = name;
        TypeFullName = typeFullName;
    }
}
