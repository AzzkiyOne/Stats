using System;
using System.Linq;

namespace Stats.RelationalOperators;

public sealed class Contains
    : IRelationalOperator<string>
{
    private Contains() { }
    public bool Eval(string lhs, string rhs) =>
        rhs
        .Split(',')
        .Any(s => lhs.Contains(s, StringComparison.CurrentCultureIgnoreCase));
    public override string ToString() => "~=";
    public static IRelationalOperator<string> Instance { get; } = new Contains();
}

public sealed class ContainsNot
    : IRelationalOperator<string>
{
    private ContainsNot() { }
    public bool Eval(string lhs, string rhs) =>
        Contains.Instance.Eval(lhs, rhs) == false;
    public override string ToString() => "!~=";
    public static IRelationalOperator<string> Instance { get; } = new ContainsNot();
}
