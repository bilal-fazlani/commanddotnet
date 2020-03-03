using System;
using System.IO;
using CommandDotNet.Rendering;

namespace CommandDotNet
{
    public static class StandardStreamReader
    {
        public static IStandardStreamReader Create(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            return new AnonymousStandardStreamReader(reader);
        }

        private class AnonymousStandardStreamReader : IStandardStreamReader
        {
            private readonly TextReader _reader;

            public AnonymousStandardStreamReader(TextReader reader)
            {
                _reader = reader;
            }

            public string ReadLine()
            {
                return _reader.ReadLine();
            }

            public string ReadToEnd()
            {
                return _reader.ReadToEnd();
            }
        }
    }
}