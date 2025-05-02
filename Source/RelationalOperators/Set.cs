using System.Collections.Generic;
using System.Linq;

namespace Stats.RelationalOperators.Set;

// TODO: Use actual sets?

public sealed class IsIn<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TRhs : IEnumerable<TLhs>
{
    private IsIn() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.Contains(lhs);
    public override string ToString() => "∈";
    public static IsIn<TLhs, TRhs> Instance { get; } = new();
}

public sealed class IsNotIn<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TRhs : IEnumerable<TLhs>
{
    private IsNotIn() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.Contains(lhs) == false;
    public override string ToString() => "∉";
    public static IsNotIn<TLhs, TRhs> Instance { get; } = new();
}

public sealed class IntersectsWith<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IntersectsWith() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.Any(lhs.Contains);
    public override string ToString() => "∩";
    public static IntersectsWith<TLhs, TRhs, TElement> Instance { get; } = new();
}

public sealed class IsSupersetOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IsSupersetOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.All(lhs.Contains);
    public override string ToString() => "⊇";
    public static IsSupersetOf<TLhs, TRhs, TElement> Instance { get; } = new();
}

public sealed class IsNotSupersetOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IsNotSupersetOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.All(lhs.Contains) == false;
    public override string ToString() => "⊅";
    public static IsNotSupersetOf<TLhs, TRhs, TElement> Instance { get; } = new();
}

public sealed class IsSubsetOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IsSubsetOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.All(rhs.Contains);
    public override string ToString() => "⊆";
    public static IsSubsetOf<TLhs, TRhs, TElement> Instance { get; } = new();
}

public sealed class IsNotSubsetOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IsNotSubsetOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.All(rhs.Contains) == false;
    public override string ToString() => "⊄";
    public static IsNotSubsetOf<TLhs, TRhs, TElement> Instance { get; } = new();
}
