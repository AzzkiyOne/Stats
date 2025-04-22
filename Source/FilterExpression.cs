using System;
using Stats.RelationalOperators;

namespace Stats;

public sealed class FilterExpression<T>
    : IFilterExpression
    where T : notnull
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
    private readonly IRelationalOperator<T> InitialOperator;
    private IRelationalOperator<T> _Operator;
    public IRelationalOperator<T> Operator
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
    public event Action<IFilterExpression>? OnChange;
    private readonly IRelationalOperator<T> InactiveStateOperator;
    // IsConstant/True?
    public bool IsActive => _Operator != InactiveStateOperator;
    public FilterExpression(
        // LhsValue?
        Func<ThingAlike, T> valueFunction,
        T initialValue,
        IRelationalOperator<T> initialOperator,
        IRelationalOperator<T> inactiveStateOperator
    )
    {
        _Value = InitialValue = initialValue;
        _Operator = InitialOperator = initialOperator;
        InactiveStateOperator = inactiveStateOperator;
        ValueFunction = valueFunction;
    }
    // Use this to avoid emitting 2 events when setting both props.
    public void Set(T value, IRelationalOperator<T> @operator)
    {
        if (_Value.Equals(value) == false || _Operator != @operator)
        {
            _Value = value;
            _Operator = @operator;
            OnChange?.Invoke(this);
        }
    }
    // Eval?
    public bool Match(ThingAlike thing)
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
    public void Reset()
    {
        Set(InitialValue, InitialOperator);
    }
}
