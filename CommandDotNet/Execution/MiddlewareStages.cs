using CommandDotNet.Parsing;

namespace CommandDotNet.Execution
{
    /// <summary>
    /// Defines stages that will exit with specific guarantees
    /// </summary>
    public enum MiddlewareStages
    {
        /// <summary>
        /// Use the <see cref="PreTransformInput"/> stage for middleware that will configure the rest of the app
        /// or need to register event handlers early in the pipeline.
        /// <br/>Guarantees:<br/>
        /// - <see cref="CommandContext.Original"/> is populated<br/>
        /// - <see cref="CommandContext.Tokens"/> is set to <see cref="OriginalInput.Tokens"/>.
        ///  The first pass of Tokenization has been performed but InputTransformations have not been applied.<br/>
        /// - <see cref="CommandContext.ExecutionConfig"/>, <see cref="CommandContext.AppSettings"/>
        ///  and <see cref="CommandContext.Console"/> are set<br/>
        /// </summary>
        PreTransformInput,

        /// <summary>
        /// In the <see cref="TransformInput"/> stage, all InputTransformations have been applied to <see cref="CommandContext.Tokens"/>.
        /// Any transformations to <see cref="CommandContext.Tokens"/> applied after this stage will not be reported by the
        /// parse directive.
        /// <br/>Guarantees:<br/>
        /// - <see cref="CommandContext.Tokens"/> is the result of all InputTransformations<br/>
        /// </summary>
        TransformInput,
        PostTransformInputPreBuild,

        /// <summary>
        /// In the <see cref="Build"/> stage commands and arguments are determined from the type specified in <see cref="AppRunner{T}"/>.
        /// <br/>Guarantees:<br/>
        /// - <see cref="CommandContext.RootCommand"/> is set for the type specified in <see cref="AppRunner{T}"/>.<br/>
        /// - All builders are prepped for the parse stage.
        ///   The entire command hierarchy may be loaded or commands may lazy load during parse.
        /// </summary>
        Build,
        PostBuildPreParseInput,

        /// <summary>
        /// In the <see cref="ParseInput"/> stage Tokens are parsed to determine the
        /// intended command, options and operands and separated arguments.
        /// <br/>Guarantees:<br/>
        /// - <see cref="CommandContext.ParseResult"/> is populated<br/>
        /// - <see cref="ParseResult.TargetCommand"/> is set to the target command<br/>
        /// - <see cref="ParseResult.ArgumentValues"/> contains the collection of values assigned to arguments<br/>
        /// - <see cref="InvocationContext.InstantiateInvocation"/> and <see cref="InvocationContext.CommandInvocation"/>
        ///  are set<br/>
        ///   - <see cref="InvocationContext.Instance"/> and <see cref="IInvocation.ParameterValues"/> are not set
        ///  until the <see cref="BindValues"/> stage.<br/>
        /// </summary>
        ParseInput,
        PostParseInputPreBindValues,

        /// <summary>
        /// In the <see cref="BindValues"/> stage the argument string values are bound to the parameters
        /// of the command method and the instance of the type containing the method is instantiated.
        /// <br/>Guarantees:<br/>
        /// - <see cref="InvocationContext.Instance"/> is set<br/>
        /// - <see cref="IInvocation.ParameterValues"/> are set<br/>
        /// </summary>
        BindValues,
        PostBindValuesPreInvoke,

        /// <summary>
        /// In the <see cref="Invoke"/> stage the <see cref="InvocationContext.CommandInvocation"/> is invoked.
        /// This is the final stage.
        /// </summary>
        Invoke
    }
}