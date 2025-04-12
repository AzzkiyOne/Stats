namespace Stats;

public interface IBinaryOp<T>
{
    bool Eval(T lhs, T rhs);
}
