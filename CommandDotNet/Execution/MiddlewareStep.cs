using JetBrains.Annotations;

namespace CommandDotNet.Execution;

[PublicAPI]
public class MiddlewareStep(MiddlewareStages stage, short? orderWithinStage = null)
{
    public static MiddlewareStep operator +(MiddlewareStep step, short increment) => 
        new(step.Stage, (short)(step.OrderWithinStage + increment));
    public static MiddlewareStep operator -(MiddlewareStep step, short decrement) =>
        new(step.Stage, (short)(step.OrderWithinStage - decrement));

    public MiddlewareStages Stage { get; } = stage;

    public short OrderWithinStage { get; } = orderWithinStage.GetValueOrDefault();

    public override string ToString() => $"{nameof(MiddlewareStep)}:{Stage} {OrderWithinStage}";

    public bool Equals(MiddlewareStep other) => Stage == other.Stage && OrderWithinStage == other.OrderWithinStage;

    public override bool Equals(object? obj) =>
        obj is not null
        && (ReferenceEquals(this, obj)
            || obj.GetType() == GetType()
            && Equals((MiddlewareStep) obj));

    public override int GetHashCode()
    {
        unchecked
        {
            return ((int)Stage * 397) ^ OrderWithinStage.GetHashCode();
        }
    }

    public static bool operator ==(MiddlewareStep left, MiddlewareStep right) => Equals(left, right);

    public static bool operator !=(MiddlewareStep left, MiddlewareStep right) => !Equals(left, right);
}