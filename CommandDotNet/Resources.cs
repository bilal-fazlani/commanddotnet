﻿using System.Diagnostics.CodeAnalysis;

namespace CommandDotNet
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public partial class Resources
    {
        public static Resources A = new Resources();

        public virtual string Common_Flag => "Flag";
        public virtual string Common_default_lc => "default";
        public virtual string Common_from_lc => "from";
        public virtual string Common_key_lc => "key";
        public virtual string Common_value_lc => "value";
        public virtual string Common_source_lc => "source";

        public virtual string Error_File_not_found(string fullPath) => $"File not found: {fullPath}";
    }
}
