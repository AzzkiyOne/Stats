using System;
using System.Collections.Generic;

namespace Stats.Widgets;

public sealed class OneToManyFilter<TObject, TOption> : NToManyFilter<TObject, TOption, TOption>
{
    new private readonly OneToManyExpression Expression;
    public OneToManyFilter(
        Func<TObject, TOption> lhs,
        IEnumerable<TOption> options,
        OptionWidgetFactory makeOptionWidget
    ) : this(new OneToManyExpression(lhs), options, makeOptionWidget)
    {
    }
    private OneToManyFilter(
        OneToManyExpression expression,
        IEnumerable<TOption> options,
        OptionWidgetFactory makeOptionWidget
    ) : base(expression, options, makeOptionWidget)
    {
        Expression = expression;
    }
    public override FilterWidget<TObject> Clone()
    {
        return new OneToManyFilter<TObject, TOption>(Expression, Options, MakeOptionWidget);
    }

    private sealed class OneToManyExpression : NtMExpression
    {
        public override IEnumerable<GenericOperator> SupportedOperators { get; } = [
            Operators.IsIn.Instance,
            Operators.IsNotIn.Instance
        ];
        public OneToManyExpression(Func<TObject, TOption> lhs) : base(lhs)
        {
        }

        private static class Operators
        {
            public sealed class IsIn : GenericOperator
            {
                private IsIn() : base("∈", "Is one of selected") { }
                public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs);
                public static IsIn Instance { get; } = new();
            }

            // ∉
            public sealed class IsNotIn : GenericOperator
            {
                private IsNotIn() : base("!∈", "Is not one of selected") { }
                public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs) == false;
                public static IsNotIn Instance { get; } = new();
            }
        }
    }
}
