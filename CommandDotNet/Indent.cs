namespace CommandDotNet
{
    /// <summary>
    /// Encapsulates logic for creating indents, providing consistency across composed reporting methods.
    /// </summary>
    public class Indent
    {
        private readonly Indent _previousDepth;
        private Indent _nextDepth;

        /// <summary>The value of a single indent</summary>
        public string SingleIndent { get; }

        /// <summary>The number of <see cref="SingleIndent"/>s assigned to this indent</summary>
        public int Depth { get; }https://github.com/bilal-fazlani/commanddotnet/pull/224/conflict?name=CommandDotNet%252FDirectives%252FParse%252FParseReporter.cs&base_oid=49cd4d0fc1e68f3ac39277916343ba029178f6a0&head_oid=f41de88e958ce53a972f2d27fb6acb60d9422125

        /// <summary>
        /// Padding is added to <see cref="Value"/>.
        /// <see cref="PadLeft"/> is passed to other <see cref="Indent"/> during <see cref="Increment"/>
        /// but is included as part of <see cref="Value"/>.
        /// </summary>
        public string PadLeft { get; }

        /// <summary>The value of <see cref="PadLeft"/> plus <see cref="SingleIndent"/> repeated <see cref="Depth"/> times</summary>
        public string Value { get; }

        private Indent(Indent previous)
        {
            SingleIndent = previous.SingleIndent;
            Depth = Depth + 1;
            Value = previous.Value + SingleIndent;
            _previousDepth = previous;
        }

        public Indent(string singleIndent = "  ", int depth = 0, string padLeft = "")
        {
            SingleIndent = singleIndent;
            Depth = depth;
            PadLeft = padLeft;
            Value = singleIndent.Repeat(depth);
        }

        /// <summary>Returns a new Indent with <see cref="Depth"/>+<see cref="count"/></summary>
        public Indent IncrementBy(int count)
        {
            var indent = this;
            for (int i = 0; i < count; i++)
            {
                indent = indent.Increment();
            }
            return indent;
        }

        /// <summary>Returns an Indent with <see cref="Depth"/>+1<br/></summary>
        public Indent Increment() => _nextDepth ?? (_nextDepth = new Indent(this));

        /// <summary>Returns an Indent with <see cref="Depth"/>-1<br/></summary>
        public Indent Decrement() => _previousDepth ?? this;

        public override string ToString()
        {
            return Value;
        }
    }
}