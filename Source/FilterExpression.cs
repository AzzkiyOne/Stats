using System;
using Stats.RelationalOperators;

namespace Stats;

// RelationalExpression? It is only used in table filters, but is it that specific?
public abstract class FilterExpression
{
    public abstract bool IsEmpty { get; }
    public abstract event Action<FilterExpression> OnChange;
    public abstract bool Eval(ThingAlike thing);
    public abstract void Reset(bool silent = false);
    // Clear?
    public abstract void NotifyChanged();
}

public sealed class FilterExpression<Lhs, Rhs> : FilterExpression where Rhs : notnull
{
    private readonly Func<ThingAlike, Lhs> LhsValueFunction;
    private readonly Rhs InitialValue;
    private Rhs _Value;
    public Rhs Value
    {
        get => _Value;
        set
        {
            if (_Value.Equals(value))
            {
                return;
            }

            _Value = value;
            OnChange?.Invoke(this);
        }
    }
    private readonly RelationalOperator<Lhs, Rhs> InitialOperator;
    private RelationalOperator<Lhs, Rhs> _Operator;
    public RelationalOperator<Lhs, Rhs> Operator
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
    private readonly RelationalOperator<Lhs, Rhs> InactiveStateOperator;
    // IsConstant/True?
    public override bool IsEmpty => _Operator == InactiveStateOperator;
    public FilterExpression(
        Func<ThingAlike, Lhs> lhsValueFunction,
        Rhs initialValue,
        RelationalOperator<Lhs, Rhs> initialOperator,
        RelationalOperator<Lhs, Rhs> inactiveStateOperator
    )
    {
        _Value = InitialValue = initialValue;
        _Operator = InitialOperator = initialOperator;
        InactiveStateOperator = inactiveStateOperator;
        LhsValueFunction = lhsValueFunction;
    }
    // Use this to avoid emitting 2 events when setting both props.
    public void Set(Rhs value, RelationalOperator<Lhs, Rhs> @operator, bool silent = false)
    {
        if (_Value.Equals(value) == false || _Operator != @operator)
        {
            _Value = value;
            _Operator = @operator;

            if (silent == false)
            {
                OnChange?.Invoke(this);
            }
        }
    }
    // Eval?
    public override bool Eval(ThingAlike thing)
    {
        try
        {
            return Operator.Eval(LhsValueFunction(thing), _Value);
        }
        catch
        {
            return false;
        }
    }
    public override void Reset(bool silent = false)
    {
        Set(InitialValue, InitialOperator);
    }
    public override void NotifyChanged()
    {
        OnChange?.Invoke(this);
    }
}
