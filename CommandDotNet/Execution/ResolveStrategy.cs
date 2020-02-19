using CommandDotNet.Builders;

namespace CommandDotNet.Execution
{
    /// <summary>
    /// Specifies whether <see cref="IDependencyResolver.Resolve"/> or <see cref="IDependencyResolver.TryResolve"/> is used.
    /// When Resolve is used, if the the Resolve method returns null instead of throwing an exception,
    /// the framework will attempt to instantiate an instance of the type.
    /// </summary>
    public enum ResolveStrategy
    {
        /// <summary>Call resolve on container.</summary>
        Resolve,
        /// <summary>Call resolve on container. If the container returns null, throw an</summary>
        ResolveOrThrow,
        TryResolve
    }
}