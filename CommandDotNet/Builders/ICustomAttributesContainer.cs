using System.Reflection;

namespace CommandDotNet.Builders
{
    public interface ICustomAttributesContainer
    {
        /// <summary>The attributes defined on the method or class that define this object</summary>
        ICustomAttributeProvider CustomAttributes { get; }
    }
}