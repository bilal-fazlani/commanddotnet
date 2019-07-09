using System;

namespace CommandDotNet
{
    public static class ContextDataExtensions
    {
        public static T GetOrAdd<T>(this IContextData contextData, Func<T> create)
        {
            var data = contextData.Get<T>();

            if (data == null)
            {
                data = create();
                contextData.Add(data);
            }

            return data;
        }
    }
}