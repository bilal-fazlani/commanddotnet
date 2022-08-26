using System;

namespace CommandDotNet.NameCasing;

public class CaseChanger
{
    private readonly Func<string, string> _changeCase;

    public CaseChanger(Func<string, string> changeCase) => 
        _changeCase = changeCase ?? throw new ArgumentNullException(nameof(changeCase));

    public string ChangeCase(string name) => _changeCase(name);
}