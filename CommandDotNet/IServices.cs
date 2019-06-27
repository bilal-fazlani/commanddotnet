using System;

namespace CommandDotNet
{
    public interface IServices
    {
        void Add<T>(T value);
        void Set<T>(T value);
        object Get(Type type);
        T Get<T>();
    }
}