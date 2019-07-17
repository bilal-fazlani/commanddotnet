using System;
using CommandDotNet.Execution;

namespace CommandDotNet.Parsing
{
    public class ParseEvents
    {
        public event Action<OnInputTransformationEventArgs> OnInputTransformation;

        public event Action<TokenizationCompletedEventArgs> OnTokenizationCompleted;

        internal void InputTransformation(CommandContext commandContext, InputTransformation transformation, TokenCollection pre, TokenCollection post)
        {
            OnInputTransformation?.Invoke(new OnInputTransformationEventArgs(commandContext, transformation, pre, post));
        }

        internal void TokenizationCompleted(CommandContext commandContext)
        {
            OnTokenizationCompleted?.Invoke(new TokenizationCompletedEventArgs(commandContext));
        }

        public class TokenizationCompletedEventArgs : EventArgs
        {
            public CommandContext CommandContext { get; }


            public TokenizationCompletedEventArgs(CommandContext commandContext)
            {
                CommandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
            }
        }

        public class OnInputTransformationEventArgs : EventArgs
        {
            public CommandContext CommandContext { get; }
            public InputTransformation Transformation { get; }
            public TokenCollection Pre { get; }
            public TokenCollection Post { get; }

            public OnInputTransformationEventArgs(
                CommandContext commandContext,
                InputTransformation transformation,
                TokenCollection pre,
                TokenCollection post)
            {
                CommandContext = commandContext ?? throw new ArgumentNullException(nameof(commandContext));
                Transformation = transformation ?? throw new ArgumentNullException(nameof(transformation));
                Pre = pre ?? throw new ArgumentNullException(nameof(pre));
                Post = post ?? throw new ArgumentNullException(nameof(post));
            }
        }
    }
}