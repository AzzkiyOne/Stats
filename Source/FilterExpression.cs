using System;
using Stats.RelationalOperators;
using Verse;

namespace Stats;

// RelationalExpression? It is only used in table filters, but is it that specific?
// Update: At this point it's more like FilterWidgetState.
public abstract class FilterExpression
{
    public abstract string OperatorString { get; }
    public abstract string RhsString { get; }
    public abstract bool IsEmpty { get; }
    public abstract event Action<FilterExpression> OnChange;
    public abstract bool Eval(ThingAlike thing);
    public abstract void Clear();
    public abstract void NotifyChanged();
}

public abstract class FilterExpression<TLhs, TRhs> : FilterExpression where TRhs : notnull
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
    public override string RhsString => _Rhs.ToString();
    private RelationalOperator<TLhs, TRhs> _Operator = EmptyOperator.Instance;
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
    public sealed override string OperatorString => _Operator.ToString();
    public sealed override event Action<FilterExpression>? OnChange;
    public sealed override bool IsEmpty => _Operator == EmptyOperator.Instance;
    public FilterExpression(Func<ThingAlike, TLhs> lhs, TRhs rhs)
    {
        Lhs = lhs;
        _Rhs = rhs;
    }
    public sealed override bool Eval(ThingAlike thing)
    {
        try
        {
            return _Operator.Eval(Lhs(thing), _Rhs);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);

            return false;
        }
    }
    public override void Clear()
    {
        Operator = EmptyOperator.Instance;
    }
    public sealed override void NotifyChanged()
    {
        OnChange?.Invoke(this);
    }

    // This operator exists only because i don't want to define Operator property as
    // nullable, because it will slow down the whole thing a bit. The table doesn't
    // evaluate empty expressions anyway.
    private sealed class EmptyOperator : RelationalOperator<TLhs, TRhs>
    {
        private EmptyOperator() { }
        public override bool Eval(TLhs lhs, TRhs rhs) => true;
        public override string ToString() => "...";
        public static EmptyOperator Instance { get; } = new();
    }
}
