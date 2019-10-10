using System;
using System.Collections.Generic;

namespace CommandDotNet
{
    /// <summary>
    /// Services and context data that can be stored with
    /// an object for later use.<br/>
    /// i.e. Middleware can store settings within the
    /// AppConfig.Services during initialization for use
    /// in the middleware pipeline.
    /// </summary>
    public interface IServices
    {
        void Add<T>(T value);
        void Add(Type type, object value);
        void AddOrUpdate<T>(T value);
        void AddOrUpdate(Type type, object value);
        T Get<T>();
        object Get(Type type);
        ICollection<KeyValuePair<Type, object>> GetAll();
    }
}