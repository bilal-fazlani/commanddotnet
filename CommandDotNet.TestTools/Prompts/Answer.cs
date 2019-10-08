using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Extensions;
using CommandDotNet.Prompts;

namespace CommandDotNet.TestTools.Prompts
{
    /// <summary>
    /// Responses for a prompt when 
    /// <see cref="IPrompter"/> or <see cref="IArgumentPrompter"/> are used
    /// </summary>
    public class Answer
    {
        public ICollection<ConsoleKeyInfo> ConsoleKeys { get; }
        public bool Reuse { get; }
        public Predicate<string> PromptFilter { get; }

        /// <summary>Constructs a response for prompt of a single value</summary>
        /// <param name="value">The response value. This is converted to an enumerable of <see cref="ConsoleKeyInfo"/>.</param>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        /// <param name="reuse">When false, this answer is discarded after use.</param>
        public Answer(string value, Predicate<string> promptFilter = null, bool reuse = false) 
            : this(value.ToConsoleKeyInfos(), promptFilter, reuse)
        {
        }

        /// <summary>Constructs a response for prompt of a list of values</summary>
        /// <param name="valueList">The response values. This is converted to an enumerable of <see cref="ConsoleKeyInfo"/> delimited by Enter.</param>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        /// <param name="reuse">When false, this answer is discarded after use.</param>
        public Answer(IEnumerable<string> valueList, Predicate<string> promptFilter = null, bool reuse = false)
            : this(valueList.SelectMany(v => v.ToConsoleKeyInfos().AppendEnterKey()), promptFilter, reuse)
        {
        }

        /// <summary>Constructs a response for prompt of a list of values</summary>
        /// <param name="consoleKeys">The <see cref="ConsoleKeyInfo"/> that represent the console input.</param>
        /// <param name="promptFilter">Applied to the prompt text. Use this to ensure the answer is for the correct prompt.</param>
        /// <param name="reuse">When false, this answer is discarded after use.</param>
        public Answer(
            IEnumerable<ConsoleKeyInfo> consoleKeys, 
            Predicate<string> promptFilter = null,
            bool reuse = false)
        {
            ConsoleKeys = consoleKeys.ToCollection();
            Reuse = reuse;
            PromptFilter = promptFilter;
        }
    }
}