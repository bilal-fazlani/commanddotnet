using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.TestTools.Prompts
{
    public class PromptResponder : IPromptResponder
    {
        private readonly List<Answer> _answers;
        private Queue<ConsoleKeyInfo> _currentAnswer;

        public PromptResponder(IEnumerable<Answer> answers)
        {
            _answers = answers.Select(a => a).ToList();
        }

        public ConsoleKeyInfo OnReadKey(TestConsole testConsole)
        {
            if (_currentAnswer != null)
            {
                if (_currentAnswer.Count == 0)
                {
                    _currentAnswer = null;
                    return ConsoleKeyInfos.EnterKey;
                }
                return _currentAnswer.Dequeue();
            }

            var promptLine = testConsole.OutLastLine;
            var answer = _answers.FirstOrDefault(a => a.PromptFilter != null && a.PromptFilter(promptLine))
                         ?? _answers.FirstOrDefault(a => a.PromptFilter == null);

            if (answer == null)
            {
                throw new Exception($"no answer available for the prompt: {promptLine}");
            }

            if (!answer.Reuse)
            {
                _answers.Remove(answer);
            }
            _currentAnswer = new Queue<ConsoleKeyInfo>(answer.ConsoleKeys);

            return OnReadKey(testConsole);
        }
    }
}