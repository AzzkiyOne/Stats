namespace Stats.RelationalOperators;

public abstract class RelationalOperator<TLhs, TRhs>
{
    public abstract bool Eval(TLhs lhs, TRhs rhs);
}
