using System;
using Stats.RelationalOperators;

namespace Stats;

public abstract class FilterExpression
{
    public abstract bool IsActive { get; }
    public abstract event Action<FilterExpression> OnChange;
    public abstract bool Match(ThingAlike thing);
    public abstract void Reset();
}

public sealed class FilterExpression<T> : FilterExpression where T : notnull
{
    private readonly Func<ThingAlike, T> ValueFunction;
    private readonly T InitialValue;
    private T _Value;
    public T Value
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
    private readonly RelationalOperator<T> InitialOperator;
    private RelationalOperator<T> _Operator;
    public RelationalOperator<T> Operator
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
    private readonly RelationalOperator<T> InactiveStateOperator;
    // IsConstant/True?
    public override bool IsActive => _Operator != InactiveStateOperator;
    public FilterExpression(
        // LhsValue?
        Func<ThingAlike, T> valueFunction,
        T initialValue,
        RelationalOperator<T> initialOperator,
        RelationalOperator<T> inactiveStateOperator
    )
    {
        _Value = InitialValue = initialValue;
        _Operator = InitialOperator = initialOperator;
        InactiveStateOperator = inactiveStateOperator;
        ValueFunction = valueFunction;
    }
    // Use this to avoid emitting 2 events when setting both props.
    public void Set(T value, RelationalOperator<T> @operator)
    {
        if (_Value.Equals(value) == false || _Operator != @operator)
        {
            _Value = value;
            _Operator = @operator;
            OnChange?.Invoke(this);
        }
    }
    // Eval?
    public override bool Match(ThingAlike thing)
    {
        try
        {
            return Operator.Eval(ValueFunction(thing), _Value);
        }
        catch
        {
            return false;
        }
    }
    public override void Reset()
    {
        Set(InitialValue, InitialOperator);
    }
}
