using System.Collections.Generic;
using System.Linq;

namespace Stats.RelationalOperators;

public sealed class Contains_Any<T>
    : RelationalOperator<IEnumerable<T>>
{
    private Contains_Any() { }
    public override bool Eval(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
        rhs.Any(item => lhs.Contains(item));
    public override string ToString() => "[Any]";
    public static RelationalOperator<IEnumerable<T>> Instance { get; } =
        new Contains_Any<T>();
}

public sealed class Contains_All<T>
    : RelationalOperator<IEnumerable<T>>
{
    private Contains_All() { }
    public override bool Eval(IEnumerable<T> lhs, IEnumerable<T> rhs) =>
        rhs.All(item => lhs.Contains(item));
    public override string ToString() => "[All]";
    public static RelationalOperator<IEnumerable<T>> Instance { get; } =
        new Contains_All<T>();
}

// "Contains only" op?
