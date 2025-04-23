using System;
using System.Linq;

namespace Stats.RelationalOperators;

public sealed class ContainsAnyOf : RelationalOperator<string, string>
{
    private ContainsAnyOf() { }
    public override bool Eval(string lhs, string rhs) =>
        rhs
        .Split(',')
        .Any(s => lhs.Contains(s, StringComparison.CurrentCultureIgnoreCase));
    public override string ToString() => "~=";
    public static RelationalOperator<string, string> Instance { get; } =
        new ContainsAnyOf();
}

public sealed class NotContainsAnyOf : RelationalOperator<string, string>
{
    private NotContainsAnyOf() { }
    public override bool Eval(string lhs, string rhs) =>
        ContainsAnyOf.Instance.Eval(lhs, rhs) == false;
    public override string ToString() => "!~=";
    public static RelationalOperator<string, string> Instance { get; } =
        new NotContainsAnyOf();
}
