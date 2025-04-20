namespace Stats.Widgets.Table.Filters;

public interface IRelationalOperator<T>
{
    bool Eval(T lhs, T rhs);
}
