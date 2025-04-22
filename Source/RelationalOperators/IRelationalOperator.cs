namespace Stats.RelationalOperators;

public interface IRelationalOperator<T>
{
    bool Eval(T lhs, T rhs);
}
