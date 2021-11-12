﻿using CommandDotNet.Extensions;
using CommandDotNet.Parsing;

namespace CommandDotNet
{
    public class ParseAppSettings : IIndentableToString
    {
        /// <summary>
        /// When false, unexpected operands will generate a parse failure.<br/>
        /// When true, unexpected arguments will be ignored and added to <see cref="ParseResult.RemainingOperands"/><br/>
        /// </summary>
        public bool IgnoreUnexpectedOperands { get; set; }
        
        public bool AllowBackslashOptionPrefix { get; set; }

        public bool AllowSingleHyphenForLongNames { get; set; }

        public override string ToString()
        {
            return ToString(new Indent());
        }

        public string ToString(Indent indent)
        {
            return this.ToStringFromPublicProperties(indent);
        }
    }
}