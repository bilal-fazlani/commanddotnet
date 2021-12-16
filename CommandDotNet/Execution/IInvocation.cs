using System.Collections.Generic;
using System.Reflection;

namespace CommandDotNet.Execution
{
    public interface IInvocation
    {
        // begin-snippet: IInvocation-properties

        /// <summary>
        /// The arguments associated with the invocation delegate.<br/>
        /// There can be multiple arguments per parameter
        /// if IArgumentModel's are used.<br/>
        /// Populated during <see cref="MiddlewareStages.ParseInput"/>
        /// </summary>
        IReadOnlyCollection<IArgument> Arguments { get; }

        /// <summary>
        /// The parameters defined in the invocation delegate.<br/>
        /// Populated during <see cref="MiddlewareStages.ParseInput"/>
        /// </summary>
        IReadOnlyCollection<ParameterInfo> Parameters { get; }

        /// <summary>
        /// The values parsed from the arguments or <see cref="CommandContext"/>
        /// if defined as a parameter.<br/>
        /// Populated during <see cref="MiddlewareStages.BindValues"/>
        /// </summary>
        object[] ParameterValues { get; }

        /// <summary>The method signature of the delegate.</summary>
        /// <remarks>
        /// Take care when assuming methods will be derived from a class.
        /// This may not always be valid in the future.<br/>
        /// Populated during <see cref="MiddlewareStages.ParseInput"/>
        /// </remarks>
        MethodInfo MethodInfo { get; }

        /// <summary>The invocation is for an interceptor method, not a command</summary>
        public bool IsInterceptor { get; }

        /// <summary>
        /// All <see cref="IArgumentModel"/>s for the invocation,
        /// flattened so you do not need to reflect properties to
        /// get all the models.
        /// </summary>
        IReadOnlyCollection<IArgumentModel> FlattenedArgumentModels { get; }

        // end-snippet

        /// <summary>Invokes the instance</summary>
        object? Invoke(CommandContext commandContext, object instance, ExecutionDelegate next);
    }
}