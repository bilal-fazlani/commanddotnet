using System;
using System.Reflection;

namespace CommandDotNet.CommandInvoker
{
    internal class DefaultCommandInvoker : ICommandInvoker
    {
        public object Invoke(CommandInvocation commandInvocation)
        {
            Type type = commandInvocation.Instance.GetType();
            MethodInfo theMethod = type.GetMethod(commandInvocation.CommandInfo.MethodName);
            object returnedObject = theMethod.Invoke(commandInvocation.Instance,commandInvocation.ParamsForCommandMethod);
            return returnedObject;
        }
    }
}
