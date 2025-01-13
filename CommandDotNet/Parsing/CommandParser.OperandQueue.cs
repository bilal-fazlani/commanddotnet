using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CommandDotNet.Parsing;

internal partial class CommandParser
{
    private class OperandQueue(IEnumerable<Operand> operands)
    {
        private readonly Queue<Operand> _operands = new(operands);
        private Operand? _listOperand;

        public bool TryDequeue([NotNullWhen(true)] out Operand? operand)
        {
            operand = Dequeue();
            return operand is not null;
        }

        private Operand? Dequeue()
        {
            // there can be only one list operand and it
            // is always the last operand
            if (_listOperand is not null) return _listOperand;
            if (_operands.Count == 0) return null;
                
            var operand = _operands.Dequeue();
            if (operand.Arity.AllowsMany())
            {
                _listOperand = operand;
            }
            return operand;
        }
    }
}