using System;
using System.Collections.Generic;
using CommandDotNet.Extensions;

namespace CommandDotNet.TestTools
{
    /// <summary>
    /// <see cref="TestCaptures"/> a convenience for testing how inputs are mapped into the command method parameters.<br/>
    /// The command class must have a public <see cref="TestCaptures"/> property for this to work.<br/>
    /// Useful for testing middleware components, not the business logic of your commands.
    /// </summary>
    public class TestCaptures
    {
        public Dictionary<Type, object> Captured { get; private set; } = new Dictionary<Type, object>();

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
            Captured.Add(value.GetType(), value);
        }

        public object Get(Type type)
        {
            return Captured.GetValueOrDefault(type);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        internal void UseOutputsFromInstance(TestCaptures testCaptures)
        {
            if (Captured.Count > 0)
                throw new InvalidOperationException(
                    $"cannot link to a {nameof(TestCaptures)} that already has outputs recorded. keys:{Captured.Keys.ToOrderedCsv()}");

            Captured = testCaptures.Captured;
        }
    }
}