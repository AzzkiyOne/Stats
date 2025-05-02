namespace Stats.RelationalOperators.Decimal;

public sealed class EqualTo : RelationalOperator<decimal, decimal>
{
    private EqualTo() { }
    public override bool Eval(decimal lhs, decimal rhs) => lhs == rhs;
    public override string ToString() => "==";
    public static EqualTo Instance { get; } = new();
}

public sealed class NotEqualTo : RelationalOperator<decimal, decimal>
{
    private NotEqualTo() { }
    public override bool Eval(decimal lhs, decimal rhs) => lhs != rhs;
    public override string ToString() => "!=";
    public static NotEqualTo Instance { get; } = new();
}

public sealed class GreaterThan : RelationalOperator<decimal, decimal>
{
    private GreaterThan() { }
    public override bool Eval(decimal lhs, decimal rhs) => lhs > rhs;
    public override string ToString() => ">";
    public static GreaterThan Instance { get; } = new();
}

public sealed class LesserThan : RelationalOperator<decimal, decimal>
{
    private LesserThan() { }
    public override bool Eval(decimal lhs, decimal rhs) => lhs < rhs;
    public override string ToString() => "<";
    public static LesserThan Instance { get; } = new();
}

public sealed class GreaterThanOrEqualTo : RelationalOperator<decimal, decimal>
{
    private GreaterThanOrEqualTo() { }
    public override bool Eval(decimal lhs, decimal rhs) => lhs >= rhs;
    public override string ToString() => ">=";
    public static GreaterThanOrEqualTo Instance { get; } = new();
}

public sealed class LesserThanOrEqualTo : RelationalOperator<decimal, decimal>
{
    private LesserThanOrEqualTo() { }
    public override bool Eval(decimal lhs, decimal rhs) => lhs <= rhs;
    public override string ToString() => "<=";
    public static LesserThanOrEqualTo Instance { get; } = new();
}
