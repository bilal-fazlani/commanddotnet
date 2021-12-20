using System;
using CommandDotNet.ClassModeling.Definitions;
using CommandDotNet.Extensions;

namespace CommandDotNet
{
    /// <summary>Arity describes how many values a user can or must provide for an argument</summary>
    public class ArgumentArity : IArgumentArity
    {
        public static readonly int Unlimited = int.MaxValue;

        // copied and modified from System.CommandLine
        // https://github.com/dotnet/command-line-api/blob/master/src/System.CommandLine/ArgumentArity.cs

        public ArgumentArity(int minimumNumberOfValues, int maximumNumberOfValues)
        {
            if (minimumNumberOfValues < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumNumberOfValues));
            }
            if (maximumNumberOfValues < minimumNumberOfValues)
            {
                throw new ArgumentException(
                    $"{nameof(maximumNumberOfValues)} must be greater than or equal to {nameof(minimumNumberOfValues)}");
            }

            Minimum = minimumNumberOfValues;
            Maximum = maximumNumberOfValues;
        }

        /// <summary>The minimum number of values the user must provide</summary>
        public int Minimum { get; }

        /// <summary>The maximum number of values the user must provide</summary>
        public int Maximum { get; }

        // begin-snippet: known-arities
        public static IArgumentArity Zero => new ArgumentArity(0, 0);
        public static IArgumentArity ZeroOrOne => new ArgumentArity(0, 1);
        public static IArgumentArity ExactlyOne => new ArgumentArity(1, 1);
        public static IArgumentArity ZeroOrMore => new ArgumentArity(0, Unlimited);
        public static IArgumentArity OneOrMore => new ArgumentArity(1, Unlimited);
        // end-snippet

        internal static IArgumentArity Default(IArgumentDef argumentDef)
        {
            return Default(
                argumentDef.Type, 
                argumentDef.IsOptional, 
                argumentDef.HasDefaultValue, 
                argumentDef.BooleanMode);
        }

        public static IArgumentArity Default(IArgument argument)
        {
            var type = argument.TypeInfo.Type;
            var defaultValue = argument.Default?.Value;
            var hasDefaultValue = argument.Services.GetOrDefault<IArgumentDef>()?.HasDefaultValue 
                                  ?? !defaultValue.IsNullValue() && !defaultValue!.IsDefaultFor(type);
            var isOptional = argument.Services.GetOrDefault<IArgumentDef>()?.IsOptional 
                             ?? argument.Arity.AllowsNone();
            return Default(type, isOptional, hasDefaultValue, argument.BooleanMode);
        }

        /// <summary>Returns the default IArgumentArity for the given type</summary>
        /// <remarks>internal for tests</remarks>
        internal static IArgumentArity Default(Type type, bool isOptional, bool hasDefaultValue, BooleanMode? booleanMode)
        {
            bool isRequired = !(isOptional || hasDefaultValue);

            if (type.IsNonStringEnumerable())
            {
                return isRequired ? OneOrMore : ZeroOrMore;
            }

            if (booleanMode == BooleanMode.Implicit && (type == typeof(bool) || type == typeof(bool?)))
            {
                return Zero;
            }

            return isRequired ? ExactlyOne : ZeroOrOne;
        }

        public override string ToString()
        {
            return $"{nameof(ArgumentArity)}:{Minimum}..{Maximum}";
        }

        // ReSharper disable once RedundantCast
        public static bool operator ==(ArgumentArity x, ArgumentArity y) => (object)x == (object)y;

        public static bool operator !=(ArgumentArity x, ArgumentArity y) => !(x == y);

        protected bool Equals(ArgumentArity other)
        {
            return Minimum == other.Minimum && Maximum == other.Maximum;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((ArgumentArity) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Minimum * 397) ^ Maximum;
            }
        }
    }
}