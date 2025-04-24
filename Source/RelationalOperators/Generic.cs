using System;

namespace Stats.RelationalOperators;

public sealed class EqualTo<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEquatable<TRhs>
{
    private EqualTo() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.Equals(rhs);
    public override string ToString() => "==";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new EqualTo<TLhs, TRhs>();
}

public sealed class NotEqualTo<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEquatable<TRhs>
{
    private NotEqualTo() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.Equals(rhs) == false;
    public override string ToString() => "!=";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new NotEqualTo<TLhs, TRhs>();
}

public sealed class GreaterThan<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TLhs : IComparable<TRhs>
{
    private GreaterThan() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.CompareTo(rhs) > 0;
    public override string ToString() => ">";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new GreaterThan<TLhs, TRhs>();
}

public sealed class LesserThan<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TLhs : IComparable<TRhs>
{
    private LesserThan() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.CompareTo(rhs) < 0;
    public override string ToString() => "<";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new LesserThan<TLhs, TRhs>();
}

public sealed class GreaterThanOrEqualTo<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TLhs : IComparable<TRhs>
{
    private GreaterThanOrEqualTo() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.CompareTo(rhs) >= 0;
    public override string ToString() => ">=";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new GreaterThanOrEqualTo<TLhs, TRhs>();
}

public sealed class LesserThanOrEqualTo<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TLhs : IComparable<TRhs>
{
    private LesserThanOrEqualTo() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.CompareTo(rhs) <= 0;
    public override string ToString() => "<=";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new LesserThanOrEqualTo<TLhs, TRhs>();
}
