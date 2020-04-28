using System;

namespace CommandDotNet.Execution
{
    public class MiddlewareStep
    {
        public static MiddlewareStep operator +(MiddlewareStep step, int increment) => 
            new MiddlewareStep(step.Stage, step.OrderWithinStage + increment);
        public static MiddlewareStep operator -(MiddlewareStep step, int decrement) =>
            new MiddlewareStep(step.Stage, step.OrderWithinStage - decrement);

        public MiddlewareStages Stage { get; }

        public int? OrderWithinStage { get; }

        public MiddlewareStep(MiddlewareStages stage, int? orderWithinStage = null)
        {
            Stage = stage;
            OrderWithinStage = orderWithinStage;
        }

        public override string ToString()
        {
            return $"{nameof(MiddlewareStep)}:{Stage} {OrderWithinStage.GetValueOrDefault()}";
        }

        protected bool Equals(MiddlewareStep other)
        {
            return Stage == other.Stage
                   && OrderWithinStage.GetValueOrDefault() == other.OrderWithinStage.GetValueOrDefault();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((MiddlewareStep)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Stage * 397) ^ OrderWithinStage.GetValueOrDefault().GetHashCode();
            }
        }

        public static bool operator ==(MiddlewareStep left, MiddlewareStep right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MiddlewareStep left, MiddlewareStep right)
        {
            return !Equals(left, right);
        }
    }
}