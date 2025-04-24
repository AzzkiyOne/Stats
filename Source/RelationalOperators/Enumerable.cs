using System.Collections.Generic;
using System.Linq;

namespace Stats.RelationalOperators;

public sealed class ContainsAnyElementOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private ContainsAnyElementOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.Any(lhs.Contains);
    public override string ToString() => "[Any]";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new ContainsAnyElementOf<TLhs, TRhs, TElement>();
}

public sealed class ContainsAllElementsOf<TLhs, TRhs, TElement> : RelationalOperator<TLhs, TRhs>
    where TLhs : IEnumerable<TElement>
    where TRhs : IEnumerable<TElement>
{
    private ContainsAllElementsOf() { }
    public override bool Eval(TLhs lhs, TRhs rhs) => rhs.All(lhs.Contains);
    public override string ToString() => "[All]";
    public static RelationalOperator<TLhs, TRhs> Instance { get; } =
        new ContainsAllElementsOf<TLhs, TRhs, TElement>();
}

// "Contains only" op?
