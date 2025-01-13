using JetBrains.Annotations;

namespace CommandDotNet.Extensions;

[PublicAPI]
public interface IIndentableToString
{
    string ToString(Indent indent);
}