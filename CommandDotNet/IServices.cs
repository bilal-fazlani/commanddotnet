using System;

namespace CommandDotNet
{
    /// <summary>Services and context data that can be captured for later use</summary>
    public interface IServices
    {
        void Add<T>(T value);
        void Set<T>(T value);
        object Get(Type type);
        T Get<T>();
    }
}