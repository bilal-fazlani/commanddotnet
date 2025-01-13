using System;

namespace CommandDotNet.TestTools;

[Serializable]
public class AssertFailedException(string message) : Exception(message);