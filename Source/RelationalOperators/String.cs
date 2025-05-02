using System;

namespace Stats.RelationalOperators.String;

public sealed class Contains : RelationalOperator<string, string>
{
    private Contains() { }
    public override bool Eval(string lhs, string rhs) =>
        lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase);
    public override string ToString() => "~=";
    public static Contains Instance { get; } = new();
}

public sealed class NotContains : RelationalOperator<string, string>
{
    private NotContains() { }
    public override bool Eval(string lhs, string rhs) =>
        lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase) == false;
    public override string ToString() => "!~=";
    public static NotContains Instance { get; } = new();
}
