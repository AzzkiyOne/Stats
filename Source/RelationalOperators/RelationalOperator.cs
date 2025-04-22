namespace Stats.RelationalOperators;

public abstract class RelationalOperator<T>
{
    public abstract bool Eval(T lhs, T rhs);
}
