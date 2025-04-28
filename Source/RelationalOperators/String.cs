using System;

namespace Stats.RelationalOperators;

public sealed class Contains : RelationalOperator<string, string>
{
    private Contains() { }
    public override bool Eval(string lhs, string rhs) =>
        lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase);
    public override string ToString() => "~=";
    public static RelationalOperator<string, string> Instance { get; } =
        new Contains();
}

public sealed class NotContains : RelationalOperator<string, string>
{
    private NotContains() { }
    public override bool Eval(string lhs, string rhs) =>
        lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase) == false;
    public override string ToString() => "!~=";
    public static RelationalOperator<string, string> Instance { get; } =
        new NotContains();
}
