using System;
using System.Collections.Generic;
using Verse;

namespace Stats.Widgets;

public abstract class FilterWidget<TObject> : Widget
{
    public virtual AbsExpression Expression { get; }
    public FilterWidget(AbsExpression expression)
    {
        Expression = expression;
    }
    public abstract FilterWidget<TObject> Clone();

    public abstract class AbsExpression
    {
        public abstract string RhsString { get; }
        public abstract bool IsEmpty { get; }
        public abstract event Action<AbsExpression> OnChange;
        public abstract bool Eval(TObject thing);
        public abstract void Clear();
        public abstract void NotifyChanged();
    }
}

public abstract class FilterWidget<T, TExprLhs, TExprRhs> : FilterWidget<T> where TExprRhs : notnull
{
    new protected GenExpression Expression { get; }
    protected FilterWidget(GenExpression expression) : base(expression)
    {
        Expression = expression;
    }

    protected abstract class GenExpression : AbsExpression
    {
        private readonly Func<T, TExprLhs> Lhs;
        private TExprRhs _Rhs;
        public TExprRhs Rhs
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
        private GenericOperator _Operator = EmptyOperator.Instance;
        public GenericOperator Operator
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
        public abstract IEnumerable<GenericOperator> SupportedOperators { get; }
        public sealed override event Action<AbsExpression>? OnChange;
        public sealed override bool IsEmpty => _Operator == EmptyOperator.Instance;
        public GenExpression(Func<T, TExprLhs> lhs, TExprRhs rhs)
        {
            Lhs = lhs;
            _Rhs = rhs;
        }
        public sealed override bool Eval(T thing)
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

        public abstract class GenericOperator
        {
            public string Symbol { get; }
            public string Description { get; }
            protected GenericOperator(string symbol = "", string description = "")
            {
                Symbol = symbol;
                Description = description;
            }
            public abstract bool Eval(TExprLhs lhs, TExprRhs rhs);
        }

        // This operator exists only because i don't want to define Operator property as
        // nullable, because it will slow down the whole thing a bit. The table doesn't
        // evaluate empty expressions anyway.
        private sealed class EmptyOperator : GenericOperator
        {
            private EmptyOperator() : base("...") { }
            public override bool Eval(TExprLhs lhs, TExprRhs rhs) => true;
            public static EmptyOperator Instance { get; } = new();
        }
    }
}
