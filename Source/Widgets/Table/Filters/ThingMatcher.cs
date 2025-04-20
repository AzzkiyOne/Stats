using System;

namespace Stats.Widgets.Table.Filters;

public class ThingMatcher<T>
    : IThingMatcher
    where T : notnull
{
    private readonly Func<ThingRec, T> ValueFunc;
    private readonly T DefaultValue;
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
    private readonly IRelationalOperator<T> DefaultOp;
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
    public event Action<IThingMatcher>? OnChange;
    public bool IsActive => _Operator is Operators.Generic.Any<T>;
    public ThingMatcher(
        Func<ThingRec, T> valueFunc,
        T defaultValue,
        IRelationalOperator<T> defaultOp
    )
    {
        _Value = DefaultValue = defaultValue;
        _Operator = DefaultOp = defaultOp;
        ValueFunc = valueFunc;
    }
    public ThingMatcher(Func<ThingRec, T> valueFunc, T defaultValue)
        : this(valueFunc, defaultValue, Operators.Generic.Any<T>.Instance)
    {
    }
    // Use this to avoid emitting 2 events when setting both props.
    public void Set(T value, IRelationalOperator<T> op)
    {
        if (_Value.Equals(value) == false || _Operator != op)
        {
            _Value = value;
            _Operator = op;
            OnChange?.Invoke(this);
        }
    }
    public bool Match(ThingRec thing)
    {
        try
        {
            return Operator.Eval(ValueFunc(thing), _Value);
        }
        catch
        {
            return false;
        }
    }
    public void Reset()
    {
        Set(DefaultValue, DefaultOp);
    }
}
