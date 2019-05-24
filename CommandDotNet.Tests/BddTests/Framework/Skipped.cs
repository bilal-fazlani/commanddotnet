using System;

namespace CommandDotNet.Tests.BddTests.Framework
{
    public class Skipped : IScenario
    {
        private readonly IScenario _backingScenario;

        public Type AppType => _backingScenario.AppType;

        public IScenarioContext Context
        {
            get => _backingScenario.Context;
            set => _backingScenario.Context = value;
        }

        public ScenarioAnd And => _backingScenario.And;

        public string WhenArgs => _backingScenario.WhenArgs;

        public ScenarioThen Then => _backingScenario.Then;

        public string SkipReason => _backingScenario.SkipReason;

        public Skipped(IScenario backingScenario = null)
        {
            _backingScenario = backingScenario;
        }

        public override string ToString()
        {
            return _backingScenario == null 
                ? "No tests skipped.  Placeholder to prevent Xunit failure"
                : _backingScenario.ToString();
        }
    }
}