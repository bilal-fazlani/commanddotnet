using System;
using CommandDotNet.Extensions;

namespace CommandDotNet;

public class LocalizationAppSettings : IIndentableToString
{
    /// <summary>When specified, this function will be used to localize user output from the framework</summary>
    public Func<string,string?>? Localize { get; set; }
    
    /// <summary>
    /// By default, the keys passed to the <see cref="Localize"/> delegate
    /// are the values define in the Resources class.<br/>
    /// Setting this to true will use the property or method names instead of the values.<br/>
    /// Example: for property - `Common_argument_lc => "argument"`<br/>
    /// the default key is "argument".<br/>
    /// When <see cref="UseMemberNamesAsKeys"/> is set to true, "Common_argument_lc" is the key.
    /// </summary>
    public bool UseMemberNamesAsKeys { get; set; }
    
    public override string ToString()
    {
        return ToString(new Indent());
    }

    public string ToString(Indent indent)
    {
        return this.ToStringFromPublicProperties(indent);
    }
}