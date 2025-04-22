using System;
using System.Linq;

namespace Stats.RelationalOperators;

public sealed class Contains
    : RelationalOperator<string>
{
    private Contains() { }
    public override bool Eval(string lhs, string rhs) =>
        rhs
        .Split(',')
        .Any(s => lhs.Contains(s, StringComparison.CurrentCultureIgnoreCase));
    public override string ToString() => "~=";
    public static RelationalOperator<string> Instance { get; } = new Contains();
}

public sealed class ContainsNot
    : RelationalOperator<string>
{
    private ContainsNot() { }
    public override bool Eval(string lhs, string rhs) =>
        Contains.Instance.Eval(lhs, rhs) == false;
    public override string ToString() => "!~=";
    public static RelationalOperator<string> Instance { get; } = new ContainsNot();
}
