using System;
using Verse;

namespace Stats.Widgets;

public abstract class FilterWidget<TObject> : Widget
{
    public abstract bool IsActive { get; }
    public abstract event Action<FilterWidget<TObject>>? OnChange;
    public abstract bool Eval(TObject @object);
    public abstract void Reset();
}

public abstract class FilterWidget<TObject, TExprLhs, TExprRhs> : FilterWidget<TObject> where TExprRhs : notnull
{
    private readonly Func<TObject, TExprLhs> Lhs;
    private TExprRhs _Rhs;
    protected TExprRhs Rhs
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
    private AbsOperator _Operator = EmptyOperator.Instance;
    protected AbsOperator Operator
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
    public sealed override event Action<FilterWidget<TObject>>? OnChange;
    public sealed override bool IsActive => _Operator != EmptyOperator.Instance;
    protected FilterWidget(Func<TObject, TExprLhs> lhs, TExprRhs rhs)
    {
        Lhs = lhs;
        _Rhs = rhs;
    }
    public sealed override bool Eval(TObject thing)
    {
        try
        {
            return _Operator.Eval(Lhs(thing), _Rhs);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);

            // Evaluate to "true", so the error would be noticeable.
            return true;
        }
    }
    public override void Reset()
    {
        Operator = EmptyOperator.Instance;
    }
    protected void NotifyChanged()
    {
        OnChange?.Invoke(this);
    }

    protected abstract class AbsOperator
    {
        public string Symbol { get; }
        public string Description { get; }
        protected AbsOperator(string symbol = "", string description = "")
        {
            Symbol = symbol;
            Description = description;
        }
        public abstract bool Eval(TExprLhs lhs, TExprRhs rhs);
    }

    // This operator exists only because i don't want to define Operator property as
    // nullable, because it will slow down the whole thing a bit. The table doesn't
    // evaluate empty expressions anyway.
    private sealed class EmptyOperator : AbsOperator
    {
        private EmptyOperator() : base("...") { }
        public override bool Eval(TExprLhs lhs, TExprRhs rhs) => true;
        public static EmptyOperator Instance { get; } = new();
    }
}
