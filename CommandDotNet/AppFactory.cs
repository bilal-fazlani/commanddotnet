using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public static class AppFactory
    {        
        public static T CreateApp<T>(Dictionary<ArguementInfo, CommandOption> construcitonParams)
        {
            object[] values = construcitonParams.Select(ValueMachine.GetValue).ToArray();   
            return (T)Activator.CreateInstance(typeof(T), values);
        }
    }
}