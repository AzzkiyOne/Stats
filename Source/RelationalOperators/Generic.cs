using System;

namespace Stats.RelationalOperators;

public sealed class Any<T>
    : RelationalOperator<T>
{
    private Any() { }
    public override bool Eval(T lhs, T rhs) => true;
    public override string ToString() => "Any";
    public static RelationalOperator<T> Instance { get; } = new Any<T>();
}

public sealed class Eq<T>
    : RelationalOperator<T>
    where T : IEquatable<T>
{
    private Eq() { }
    public override bool Eval(T lhs, T rhs) => lhs.Equals(rhs);
    public override string ToString() => "==";
    public static RelationalOperator<T> Instance { get; } = new Eq<T>();
}

public sealed class EqNot<T>
    : RelationalOperator<T>
    where T : IEquatable<T>
{
    private EqNot() { }
    public override bool Eval(T lhs, T rhs) => lhs.Equals(rhs) == false;
    public override string ToString() => "!=";
    public static RelationalOperator<T> Instance { get; } = new EqNot<T>();
}

public sealed class Gt<T>
    : RelationalOperator<T>
    where T : IComparable<T>
{
    private Gt() { }
    public override bool Eval(T lhs, T rhs) => lhs.CompareTo(rhs) > 0;
    public override string ToString() => ">";
    public static RelationalOperator<T> Instance { get; } = new Gt<T>();
}

public sealed class Lt<T>
    : RelationalOperator<T>
    where T : IComparable<T>
{
    private Lt() { }
    public override bool Eval(T lhs, T rhs) => lhs.CompareTo(rhs) < 0;
    public override string ToString() => "<";
    public static RelationalOperator<T> Instance { get; } = new Lt<T>();
}

public sealed class GtOrEq<T>
    : RelationalOperator<T>
    where T : IComparable<T>
{
    private GtOrEq() { }
    public override bool Eval(T lhs, T rhs) => lhs.CompareTo(rhs) >= 0;
    public override string ToString() => ">=";
    public static RelationalOperator<T> Instance { get; } = new GtOrEq<T>();
}

public sealed class LtOrEq<T>
    : RelationalOperator<T>
    where T : IComparable<T>
{
    private LtOrEq() { }
    public override bool Eval(T lhs, T rhs) => lhs.CompareTo(rhs) <= 0;
    public override string ToString() => "<=";
    public static RelationalOperator<T> Instance { get; } = new LtOrEq<T>();
}
