using System.Collections.Generic;
using System.Linq;

namespace Stats.RelationalOperators;

public sealed class ContainsAnyElementOf<Lhs, Rhs, ItemType> : RelationalOperator<Lhs, Rhs>
    where Lhs : IEnumerable<ItemType>
    where Rhs : IEnumerable<ItemType>
{
    private ContainsAnyElementOf() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => rhs.Any(lhs.Contains);
    public override string ToString() => "[Any]";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } =
        new ContainsAnyElementOf<Lhs, Rhs, ItemType>();
}

public sealed class ContainsAllElementsOf<Lhs, Rhs, ItemType> : RelationalOperator<Lhs, Rhs>
    where Lhs : IEnumerable<ItemType>
    where Rhs : IEnumerable<ItemType>
{
    private ContainsAllElementsOf() { }
    public override bool Eval(Lhs lhs, Rhs rhs) => rhs.All(lhs.Contains);
    public override string ToString() => "[All]";
    public static RelationalOperator<Lhs, Rhs> Instance { get; } =
        new ContainsAllElementsOf<Lhs, Rhs, ItemType>();
}

// "Contains only" op?
