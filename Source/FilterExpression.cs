using System;
using Stats.RelationalOperators;

namespace Stats;

// RelationalExpression? It is only used in table filters, but is it that specific?
public abstract class FilterExpression
{
    public abstract bool IsEmpty { get; }
    public abstract event Action<FilterExpression> OnChange;
    public abstract bool Eval(ThingAlike thing);
    public abstract void Clear(bool silent = false);
    public abstract void NotifyChanged();
}

public sealed class FilterExpression<TLhs, TRhs> : FilterExpression where TRhs : notnull
{
    private readonly Func<ThingAlike, TLhs> Lhs;
    private TRhs _Rhs;
    public TRhs Rhs
    {
        get => _Rhs;
        set
        {
            if (_Rhs.Equals(value))
            {
                return;
            }

            _Rhs = value;
            OnChange?.Invoke(this);
        }
    }
    private RelationalOperator<TLhs, TRhs> _Operator;
    public RelationalOperator<TLhs, TRhs> Operator
    {
        get => _Operator;
        set
        {
            if (_Operator == value)
            {
                return;
            }

            _Operator = value;
            OnChange?.Invoke(this);
        }
    }
    public override event Action<FilterExpression>? OnChange;
    public override bool IsEmpty => _Operator == EmptyOperator.Instance;
    public FilterExpression(
        Func<ThingAlike, TLhs> lhs,
        TRhs rhs,
        RelationalOperator<TLhs, TRhs> @operator
    )
    {
        Lhs = lhs;
        _Rhs = rhs;
        _Operator = @operator;
    }
    public FilterExpression(Func<ThingAlike, TLhs> lhs, TRhs rhs)
        : this(lhs, rhs, EmptyOperator.Instance)
    {
    }
    // Use this to avoid emitting 2 events when setting both props.
    public void Set(TRhs rhs, RelationalOperator<TLhs, TRhs> @operator)
    {
        if (_Rhs.Equals(rhs) == false || _Operator != @operator)
        {
            _Rhs = rhs;
            _Operator = @operator;

            OnChange?.Invoke(this);
        }
    }
    public override bool Eval(ThingAlike thing)
    {
        // TODO: This may be redundant.
        try
        {
            return Operator.Eval(Lhs(thing), _Rhs);
        }
        catch
        {
            return false;
        }
    }
    public override void Clear(bool silent = false)
    {
        if (silent)
        {
            _Operator = EmptyOperator.Instance;
        }
        else
        {
            Operator = EmptyOperator.Instance;
        }
    }
    public override void NotifyChanged()
    {
        OnChange?.Invoke(this);
    }

    // This operator exists only because i don't want to define Operator property as
    // nullable, because it will slow down the whole thing a bit. The table doesn't
    // evaluate empty expressions anyway.
    public sealed class EmptyOperator : RelationalOperator<TLhs, TRhs>
    {
        private EmptyOperator() { }
        public override bool Eval(TLhs lhs, TRhs rhs) => true;
        public override string ToString() => "...";
        public static RelationalOperator<TLhs, TRhs> Instance { get; } =
            new EmptyOperator();
    }
}
