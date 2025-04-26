using System.Collections.Generic;
using System.Linq;

namespace Stats.RelationalOperators;

public sealed class IsIn<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TRhs : IEnumerable<TLhs>
{
    private IsIn() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.Contains(lhs);
    public override string ToString() => "∈";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new IsIn<TLhs, TRhs>();
}

public sealed class IsNotIn<TLhs, TRhs> : RelationalOperator<TLhs, TRhs>
    where TRhs : IEnumerable<TLhs>
{
    private IsNotIn() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.Contains(lhs) == false;
    public override string ToString() => "∉";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new IsNotIn<TLhs, TRhs>();
}

public sealed class IntersectsWith<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IntersectsWith() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.Any(lhs.Contains);
    public override string ToString() => "∩";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new IntersectsWith<TLhs, TRhs, TElement>();
}

public sealed class IsSupersetOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IsSupersetOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.All(lhs.Contains);
    public override string ToString() => "⊃";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new IsSupersetOf<TLhs, TRhs, TElement>();
}

public sealed class IsNotSupersetOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IsNotSupersetOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.All(lhs.Contains) == false;
    public override string ToString() => "⊅";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new IsNotSupersetOf<TLhs, TRhs, TElement>();
}

public sealed class IsSubsetOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IsSubsetOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.All(rhs.Contains);
    public override string ToString() => "⊆";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new IsSubsetOf<TLhs, TRhs, TElement>();
}

public sealed class IsNotSubsetOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private IsNotSubsetOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => lhs.All(rhs.Contains) == false;
    public override string ToString() => "⊄";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new IsNotSubsetOf<TLhs, TRhs, TElement>();
}
