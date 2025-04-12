using System;

namespace Stats;

public abstract class ThingMatcher<T>
    : IThingMatcher
    where T : IEquatable<T>
{
    private readonly Func<ThingRec, T> ValueFunc;
    private readonly T DefaultValue;
    private T _Value;
    public T Value
    {
        get => _Value;
        set
        {
            if (_Value.Equals(value)) return;

            _Value = value;
            OnChange?.Invoke();
        }
    }
    private readonly IBinaryOp<T> DefaultOp;
    private IBinaryOp<T> _Operator;
    public IBinaryOp<T> Operator
    {
        get => _Operator;
        set
        {
            if (_Operator == value) return;

            _Operator = value;
            OnChange?.Invoke();
        }
    }
    public event Action? OnChange;
    public ThingMatcher(
        T defaultValue,
        IBinaryOp<T> defaultOp,
        Func<ThingRec, T> valueFunc
    )
    {
        _Value = DefaultValue = defaultValue;
        _Operator = DefaultOp = defaultOp;
        ValueFunc = valueFunc;
    }
    // Use this to avoid emitting 2 events when setting both props.
    public void Set(T value, IBinaryOp<T> op)
    {
        if (_Value.Equals(value) == false || _Operator != op)
        {
            _Value = value;
            _Operator = op;
            OnChange?.Invoke();
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
