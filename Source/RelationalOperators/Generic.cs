using System;

namespace Stats.RelationalOperators;

public sealed class Any<T>
    : IRelationalOperator<T>
{
    private Any() { }
    public bool Eval(T lhs, T rhs) => true;
    public override string ToString() => "Any";
    public static IRelationalOperator<T> Instance { get; } = new Any<T>();
}

public sealed class Eq<T>
    : IRelationalOperator<T>
    where T : IEquatable<T>
{
    private Eq() { }
    public bool Eval(T lhs, T rhs) => lhs.Equals(rhs);
    public override string ToString() => "==";
    public static IRelationalOperator<T> Instance { get; } = new Eq<T>();
}

public sealed class EqNot<T>
    : IRelationalOperator<T>
    where T : IEquatable<T>
{
    private EqNot() { }
    public bool Eval(T lhs, T rhs) => lhs.Equals(rhs) == false;
    public override string ToString() => "!=";
    public static IRelationalOperator<T> Instance { get; } = new EqNot<T>();
}

public sealed class Gt<T>
    : IRelationalOperator<T>
    where T : IComparable<T>
{
    private Gt() { }
    public bool Eval(T lhs, T rhs) => lhs.CompareTo(rhs) > 0;
    public override string ToString() => ">";
    public static IRelationalOperator<T> Instance { get; } = new Gt<T>();
}

public sealed class Lt<T>
    : IRelationalOperator<T>
    where T : IComparable<T>
{
    private Lt() { }
    public bool Eval(T lhs, T rhs) => lhs.CompareTo(rhs) < 0;
    public override string ToString() => "<";
    public static IRelationalOperator<T> Instance { get; } = new Lt<T>();
}

public sealed class GtOrEq<T>
    : IRelationalOperator<T>
    where T : IComparable<T>
{
    private GtOrEq() { }
    public bool Eval(T lhs, T rhs) => lhs.CompareTo(rhs) >= 0;
    public override string ToString() => ">=";
    public static IRelationalOperator<T> Instance { get; } = new GtOrEq<T>();
}

public sealed class LtOrEq<T>
    : IRelationalOperator<T>
    where T : IComparable<T>
{
    private LtOrEq() { }
    public bool Eval(T lhs, T rhs) => lhs.CompareTo(rhs) <= 0;
    public override string ToString() => "<=";
    public static IRelationalOperator<T> Instance { get; } = new LtOrEq<T>();
}
