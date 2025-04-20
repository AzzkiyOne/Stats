namespace Stats.Widgets.Table.Filters.Operators;

public interface IRelationalOperator<T>
{
    bool Eval(T lhs, T rhs);
}
