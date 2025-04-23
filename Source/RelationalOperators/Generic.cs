using System;

namespace Stats.RelationalOperators;

public sealed class Any<Lhs, Rhs> : RelationalOperator<Lhs, Rhs>
{
    private Any() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => true;
    public override string ToString() => "Any";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } = new Any<Lhs, Rhs>();
}

public sealed class Equals<Lhs, Rhs> : RelationalOperator<Lhs, Rhs>
    where Lhs : IEquatable<Rhs>
{
    private Equals() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => lhs.Equals(rhs);
    public override string ToString() => "==";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } =
        new Equals<Lhs, Rhs>();
}

public sealed class NotEquals<Lhs, Rhs> : RelationalOperator<Lhs, Rhs>
    where Lhs : IEquatable<Rhs>
{
    private NotEquals() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => lhs.Equals(rhs) == false;
    public override string ToString() => "!=";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } =
        new NotEquals<Lhs, Rhs>();
}

public sealed class GreaterThan<Lhs, Rhs> : RelationalOperator<Lhs, Rhs>
    where Lhs : IComparable<Rhs>
{
    private GreaterThan() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => lhs.CompareTo(rhs) > 0;
    public override string ToString() => ">";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } =
        new GreaterThan<Lhs, Rhs>();
}

public sealed class LesserThan<Lhs, Rhs> : RelationalOperator<Lhs, Rhs>
    where Lhs : IComparable<Rhs>
{
    private LesserThan() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => lhs.CompareTo(rhs) < 0;
    public override string ToString() => "<";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } =
        new LesserThan<Lhs, Rhs>();
}

public sealed class GreaterThanOrEquals<Lhs, Rhs> : RelationalOperator<Lhs, Rhs>
    where Lhs : IComparable<Rhs>
{
    private GreaterThanOrEquals() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => lhs.CompareTo(rhs) >= 0;
    public override string ToString() => ">=";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } =
        new GreaterThanOrEquals<Lhs, Rhs>();
}

public sealed class LesserThanOrEquals<Lhs, Rhs> : RelationalOperator<Lhs, Rhs>
    where Lhs : IComparable<Rhs>
{
    private LesserThanOrEquals() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => lhs.CompareTo(rhs) <= 0;
    public override string ToString() => "<=";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } =
        new LesserThanOrEquals<Lhs, Rhs>();
}
