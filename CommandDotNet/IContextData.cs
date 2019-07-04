using System;

namespace CommandDotNet
{
    public interface IContextData
    {
        void Add<T>(T value);
        void Set<T>(T value);
        object Get(Type type);
        T Get<T>();
    }
}