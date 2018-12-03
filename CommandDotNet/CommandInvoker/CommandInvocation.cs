using System.Collections.Generic;
using CommandDotNet.Models;

namespace CommandDotNet.CommandInvoker
{
    public class CommandInvocation
    {
        public CommandInfo CommandInfo { get; set; }

        public List<ArgumentInfo> ArgsFromCli { get; set; }

        public object[] ParamsForCommandMethod { get; set; }

        public object Instance { get; set; }
    }
}
