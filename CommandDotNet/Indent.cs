namespace CommandDotNet
{
    /// <summary>
    /// Encapsulates logic for creating indents, providing consistency across composed reporting methods.
    /// </summary>
    public class Indent
    {
        private Indent _nextDepth;

        /// <summary>The value of a single indent</summary>
        public string SingleIndent { get; }

        /// <summary>The number of <see cref="SingleIndent"/>s assigned to this indent</summary>
        public int Depth { get; }

        /// <summary>The value of <see cref="SingleIndent"/> repeated <see cref="Depth"/> times</summary>
        public string Value { get; }

        /// <summary>Returns a new Indent with <see cref="Depth"/>+1<br/></summary>
        public Indent NextDepth => _nextDepth ?? (_nextDepth = new Indent(SingleIndent, Depth+1));

        public Indent(string singleIndent = "  ", int depth = 0)
        {
            SingleIndent = singleIndent;
            Depth = depth;
            Value = singleIndent.Repeat(depth);
        }

        /// <summary>Returns a new Indent with <see cref="Depth"/>+<see cref="by"/></summary>
        public Indent Deeper(int by)
        {
            var indent = this;
            for (int i = 0; i < by; i++)
            {
                indent = indent.NextDepth;
            }
            return indent;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}