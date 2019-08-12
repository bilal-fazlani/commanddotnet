using System;

namespace CommandDotNet
{
    public static class ContextDataExtensions
    {
        public static T GetOrAdd<T>(this IServices services, Func<T> create)
        {
            var data = services.Get<T>();

            if (data == null)
            {
                data = create();
                services.Add(data);
            }

            return data;
        }
    }
}