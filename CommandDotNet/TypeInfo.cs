using System;

namespace CommandDotNet
{
    /// <summary>TypeInfo is used to determine how to parse inputs and display help</summary>
    public class TypeInfo : ITypeInfo
    {
        /// <summary>A bool with no display name</summary>
        public static readonly TypeInfo Flag = new TypeInfo(typeof(bool), typeof(bool));

        /// <summary>The type of the property or parameter defining an argument</summary>
        public Type Type { get; }

        /// <summary>
        /// When <see cref="Type"/> is a generic type (Nullable or IEnumerable) then <see cref="UnderlyingType"/>
        /// is the first generic argument of that type.
        /// </summary>
        public Type UnderlyingType { get; }

        /// <summary>The name to display in help</summary>
        public string DisplayName { get; set; }

        public TypeInfo(Type type, Type underlyingType, string displayName = null)
        {            
            Type = type ?? throw new ArgumentNullException(nameof(type));
            UnderlyingType = underlyingType ?? throw new ArgumentNullException(nameof(underlyingType));
            DisplayName = displayName;
        }
    }
}