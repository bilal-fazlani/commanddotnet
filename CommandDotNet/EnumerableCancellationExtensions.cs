using System;
using System.Collections.Generic;
using System.Threading;

namespace CommandDotNet
{
    public static class EnumerableCancellationExtensions
    {
        /// <summary>
        /// Enumerates the items until the <see cref="CancellationToken"/> is cancelled.
        /// Does not throw <see cref="OperationCanceledException"/>.<br/>
        /// BEWARE: this is not async safe when used with <see cref="sleepIntervalInMs"/>.
        /// </summary>
        public static IEnumerable<T> UntilCancelled<T>(this IEnumerable<T> items, CancellationToken ct, int sleepIntervalInMs = 0)
        {
            foreach (var item in items)
            {
                if (ct.IsCancellationRequested) yield break;
                yield return item;
                if (sleepIntervalInMs > 0) ct.WaitHandle.WaitOne(sleepIntervalInMs);
            }
        }

        /// <summary>
        /// Enumerates the items until the <see cref="CancellationToken"/> is cancelled.
        /// Throws <see cref="OperationCanceledException"/>.<br/>
        /// BEWARE: this is not async safe when used with <see cref="sleepIntervalInMs"/>.
        /// </summary>
        /// <exception cref="OperationCanceledException"></exception>
        public static IEnumerable<T> ThrowIfCancelled<T>(this IEnumerable<T> items, CancellationToken ct, int sleepIntervalInMs = 0)
        {
            foreach (var item in items)
            {
                ct.ThrowIfCancellationRequested();
                yield return item;
                if (sleepIntervalInMs > 0) ct.WaitHandle.WaitOne(sleepIntervalInMs);
            }
        }
    }
}