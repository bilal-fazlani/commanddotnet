using System;
using System.Collections.Generic;

namespace CommandDotNet.Tests.Utils
{
    public class TestOutputs
    {
        public Dictionary<Type, object> Outputs { get; } = new Dictionary<Type, object>();

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
    }
}