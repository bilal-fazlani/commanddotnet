using System;

namespace CommandDotNet.Execution
{
    public class MiddlewareStep
    {
        public static MiddlewareStep operator +(MiddlewareStep step, int increment) => 
            new MiddlewareStep(step.Stage, step.OrderWithinStage + increment);
        public static MiddlewareStep operator -(MiddlewareStep step, int decrement) =>
            new MiddlewareStep(step.Stage, step.OrderWithinStage + decrement);

        public MiddlewareStages Stage { get; }

        public int? OrderWithinStage { get; }

        [Obsolete("Use OrderWithinStage instead")]
        public int Order { get; }

        public MiddlewareStep(MiddlewareStages stage, int? orderWithinStage = null)
        {
            Stage = stage;
            OrderWithinStage = orderWithinStage;
        }
    }
}