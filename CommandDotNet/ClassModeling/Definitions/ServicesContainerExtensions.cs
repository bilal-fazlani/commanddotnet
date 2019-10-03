namespace CommandDotNet.ClassModeling.Definitions
{
    internal static class ServicesContainerExtensions
    {
        internal static ICommandDef GetCommandDef(this Command servicesContainer) =>
            servicesContainer.Services.Get<ICommandDef>();

        internal static IArgumentDef GetArgumentDef(this IArgument servicesContainer) =>
            servicesContainer.Services.Get<IArgumentDef>();
    }
}