using System;
using System.Runtime.Serialization;

namespace CommandDotNet.TestTools
{
    [Serializable]
    public class AssertFailedException(string message) : Exception(message);
}