using System;
using System.Collections.Generic;
using System.Linq;
using CommandDotNet.Models;
using Microsoft.Extensions.CommandLineUtils;

namespace CommandDotNet
{
    public static class AppFactory
    {                
        public static object CreateApp(Type type, Dictionary<ArgumentInfo, CommandOption> construcitonParams = null)
        {

            construcitonParams = construcitonParams ?? new Dictionary<ArgumentInfo, CommandOption>();
            
            object[] values = construcitonParams.Select(ValueMachine.GetValue).ToArray();   
            return Activator.CreateInstance(type, values);
        }
    }
}