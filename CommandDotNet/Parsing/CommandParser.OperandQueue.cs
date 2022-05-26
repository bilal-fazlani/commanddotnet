using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CommandDotNet.Parsing
{
    internal partial class CommandParser
    {
        private class OperandQueue
        {
            private readonly Queue<Operand> _operands;
            private Operand? _listOperand;

            public OperandQueue(IEnumerable<Operand> operands)
            {
                _operands = new Queue<Operand>(operands);
            }

            public bool TryDequeue([NotNullWhen(true)] out Operand? operand)
            {
                operand = Dequeue();
                return operand is { };
            }

            public Operand? Dequeue()
            {
                // there can be only one list operand and it
                // is always the last operand
                if (_listOperand is { })
                {
                    return _listOperand;
                }
                if (_operands.Any())
                {
                    var operand = _operands.Dequeue();
                    if (operand.Arity.AllowsMany())
                    {
                        _listOperand = operand;
                    }
                    return operand;
                }
                return null;
            }
        }
    }
}