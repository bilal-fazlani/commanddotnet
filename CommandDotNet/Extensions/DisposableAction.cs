using System;

namespace CommandDotNet.Extensions;

internal class DisposableAction : IDisposable
{
    internal static DisposableAction Null { get; } = new();

    private readonly Action? _action;

    private DisposableAction()
    {
    }

    public DisposableAction(Action action) => _action = action.ThrowIfNull();

    public void Dispose() => _action?.Invoke();
}