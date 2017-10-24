using System;
using Xunit;

namespace CommandDotNet.Tests
{
    public class MyCommands
    {
        public void Paint(string color)
        {
            Console.WriteLine("***********************************\r\n" + color + "\r\n***********************************");
        }
    }
}