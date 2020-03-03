namespace CommandDotNet.Builders
{
    public interface IServicesContainer
    {
        /// <summary>The services used by middleware and associated with this object</summary>
        IServices Services { get; }
    }
}