using CommandDotNet.LocalizationExample.Interfaces.Commands;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandDotNet.LocalizationExample.Commands
{

    [Command("git", Description = "Git_Description")]
    public class Git : IGit
    {
        private readonly IStringLocalizer<Git> _Localizer;
        public Git(IStringLocalizer<Git> localizer)
        {
            _Localizer = localizer;
        }

        [Command("commit", 
            Description = "Git_Commit_Description",
            ExtendedHelpText = "Git_Commit_ExtendedHelpText")]
        public void Commit([Option('m')] string commitMessage)
        {
            Console.WriteLine(_Localizer.GetString("Commit_Message",commitMessage));
        }
    }
}
