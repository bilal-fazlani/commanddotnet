using System;
using System.Collections.Generic;
using CommandDotNet.Models;

namespace CommandDotNet
{
    public class RunContext
    {
        public Func<object[], object> RunDelegate { get; set; }
        public AppSettings AppSettings { get; set; }
        public CommandInfo CommandInfo { get; set; }
        public List<ArgumentInfo> ParameterValues { get; set; }
        public object[] MergedParameters { get; set; }
    }
}