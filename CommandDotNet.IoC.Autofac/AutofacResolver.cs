﻿using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using CommandDotNet.Builders;

namespace CommandDotNet.IoC.Autofac
{
    public class AutofacResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public AutofacResolver(IContainer container)
        {
            _container = container;
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public bool TryResolve(Type type, [NotNullWhen(true)] out object? item)
        {
            return _container.TryResolve(type, out item);
        }
    }
}