using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools
{
    public class TestOutputs
    {
        public Dictionary<Type, object> Outputs { get; private set; } = new Dictionary<Type, object>();

        public void CaptureIfNotNull(object value)
        {
            if (value == null)
            {
                return;
            }

            Capture(value);
        }

        public void Capture(object value)
        {
            // arguments should only be captured once.  don't allow overwrites.
            Outputs.Add(value.GetType(), value);
        }

        public object Get(Type type)
        {
            return Outputs.GetValueOrDefault(type);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        internal void UseOutputsFromInstance(TestOutputs testOutputs)
        {
            if (Outputs.Count > 0)
                throw new InvalidOperationException(
                    $"cannot link to a {nameof(TestOutputs)} that already has outputs recorded. keys:{Outputs.Keys.ToOrderedCsv()}");

            Outputs = testOutputs.Outputs;
        }
    }
}