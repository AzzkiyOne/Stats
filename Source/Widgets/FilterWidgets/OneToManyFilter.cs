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
        public override IEnumerable<GenOperator> SupportedOperators => [
            Operators.IsIn.Instance,
            Operators.IsNotIn.Instance
        ];
        public OneToManyExpression(Func<TObject, TOption> lhs) : base(lhs)
        {
        }

        private static class Operators
        {
            public sealed class IsIn : GenOperator
            {
                private IsIn() { }
                public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs);
                public override string ToString() => "∈";
                public static IsIn Instance { get; } = new();
            }

            public sealed class IsNotIn : GenOperator
            {
                private IsNotIn() { }
                public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs) == false;
                public override string ToString() => "∉";
                public static IsNotIn Instance { get; } = new();
            }
        }
    }
}
