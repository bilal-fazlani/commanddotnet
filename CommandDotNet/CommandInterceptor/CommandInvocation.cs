using System.Collections.Generic;
using CommandDotNet.Models;

namespace CommandDotNet.CommandInterceptor
{
    public class CommandInvocation
    {
        public CommandInfo CommandInfo { get; set; }

        public List<ArgumentInfo> ParameterValues { get; set; }

        public object[] MergedParameters { get; set; }

        public object Instance { get; set; }
    }
}
