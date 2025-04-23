namespace Stats.RelationalOperators;

public abstract class RelationalOperator<Lhs, Rhs>
{
    public abstract bool Eval(Lhs lhs, Rhs rhs);
}
