using CommandDotNet.LocalizationExample.Interfaces.Commands;

namespace CommandDotNet.LocalizationExample.Commands
{
    public class Menu : IMenu
    {
        [Subcommand]
        public Git? Git_Command { get; set; }
    }
}
