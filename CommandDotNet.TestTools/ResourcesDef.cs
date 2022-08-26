using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandDotNet.Extensions;
using static System.Environment;

namespace CommandDotNet.TestTools
{
    public class ResourcesDef
    {
        public static ResourcesDef Parse<T>() => new(typeof(T));

        public ResourcesDef(Type type)
        {
            Properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .ToCollection();
            Methods = type
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName) // exclude property methods
                .ToCollection();

            Type = type;
            IsProxy = Type.BaseType != null && Type.GetConstructors().Any(c =>
            {
                var parameters = c.GetParameters();
                return parameters.Length == 2
                       && parameters.First().ParameterType == typeof(Func<string, string?>)
                       && parameters.Last().ParameterType == typeof(bool);
            });
        }

        public bool IsProxy { get; }
        public Type Type { get; }
        public ICollection<PropertyInfo> Properties { get; }
        public ICollection<MethodInfo> Methods { get; }

        public object? NewProxyInstance(bool memberNameAsKey = false)
        {
            return IsProxy
                ? Activator.CreateInstance(Type, (Func<string, string?>) (s => s), memberNameAsKey)
                : throw new Exception($"type:{Type.FullName} is not a proxy");
        }

        public IEnumerable<(string error, ICollection<MemberInfo> members)> Validate()
        {
            var invalid = Methods
                .Where(m => m.GetParameters().Any(p => p.ParameterType != typeof(string)))
                .Cast<MemberInfo>()
                .ToList();
            
            if (invalid.Any())
            {
                yield return ("Proxy method parameters must be of type string", invalid);
            }
        }

        public IEnumerable<string> IsMissingMembersFrom(ResourcesDef source)
        {
            var proxy = this;
            
            var missingProperties = source.Properties
                .Where(m => proxy.Properties.All(pm => pm.Name != m.Name))
                .Select(m => m.FullName(true));
                
            var missingMethods = source.Methods
                .Where(m => proxy.Methods.All(pm => pm.Name != m.Name))
                .Select(m => m.FullName(true));

            return missingProperties.Concat(missingMethods);
        }

        public IEnumerable<(MemberInfo member, string value)> GetMembersWithDefaults()
        {
            var proxy = NewProxyInstance();
            foreach (var property in Properties)
            {
                // should never be null because Proxy will use 
                // base member if the null was returned from localize function
                yield return (property, (string)property.GetValue(proxy)!);
            }
            foreach (var method in Methods)
            {
                var placeHolders = method.GetParameters()
                    .Select((_, i) => $"{{{i}}}")
                    .Cast<object>()
                    .ToArray();

                // should never be null because Proxy will use 
                // base member if the null was returned from localize function
                yield return (method, (string)method.Invoke(proxy, placeHolders)!);
            }
        }

        public string? GenerateProxyClass(string className, bool valueAsKey)
        {
            var proxyCode = new StringBuilder();

            foreach (var property in Properties.Select(p => p.Name))
            {
                var localize = "_localize(_memberNameAsKey " + NewLine +
                               $"                ? \"{property}\"" + NewLine +
                               $"                : base.{property})";

                proxyCode.AppendLine(NewLine +
                    $"        public override string {property} =>{NewLine}" +
                    $"            {localize}{NewLine}" +
                    $"            ?? base.{property};");
            }

            foreach (var method in Methods)
            {
                var parameters = method.GetParameters();
                var paramDef = parameters.
                    Select(p => $"string{(p.IsNullable() ? "?" : "")} {p.Name}")
                    .ToCsv(", ");
                var paramPlaceHolders = parameters
                    .Select((_, i) => $"\"{{{i}}}\"")
                    .ToCsv(", ");
                var paramPassThrus = parameters
                    .Select(p => $"{p.Name}")
                    .ToCsv(", ");

                var localize = "_localize(_memberNameAsKey " + NewLine +
                               $"                ? \"{method.Name}\"" + NewLine +
                               $"                : base.{method.Name}({paramPlaceHolders}))";

                localize = paramPassThrus.IsNullOrEmpty()
                    ? localize
                    : $"Format({localize},{NewLine}                {paramPassThrus})";
                
                proxyCode.AppendLine(NewLine +
                    $"        public override string {method.Name}({paramDef}) =>{NewLine}" +
                    $"            {localize}{NewLine}" +
                    $"            ?? base.{method.Name}({paramPassThrus});");
            }

            var classDef = @$"
using System;

namespace {Type.Namespace}
{{
    // this class generated by {GetType().Name}.{nameof(GenerateProxyClass)}
    public class {className} : Resources
    {{
        private readonly Func<string, string?> _localize;
        private readonly bool _memberNameAsKey;

        public {className}(Func<string, string?> localize, bool memberNameAsKey = false)
        {{
            _localize = localize ?? throw new ArgumentNullException(nameof(localize));
            _memberNameAsKey = memberNameAsKey;
        }}

        private static string? Format(string? value, params object?[] args) =>
            value is null ? null : string.Format(value, args);
{proxyCode}
    }}
}}";
            return classDef;
        }
        
        public override string ToString() => Type.FullName!;
    }
}