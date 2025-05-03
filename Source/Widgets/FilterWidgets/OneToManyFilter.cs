using System;
using System.Collections.Generic;

namespace Stats.Widgets.FilterWidgets;

public sealed class OneToManyFilter<TOption> : NToManyFilter<TOption, TOption>
{
    new private readonly OneToManyExpression Expression;
    public OneToManyFilter(
        Func<ThingAlike, TOption> lhs,
        IEnumerable<TOption> options,
        OptionWidgetFactory<TOption> makeOptionWidget
    ) : this(new OneToManyExpression(lhs), options, makeOptionWidget)
    {
    }
    private OneToManyFilter(
        OneToManyExpression expression,
        IEnumerable<TOption> options,
        OptionWidgetFactory<TOption> makeOptionWidget
    ) : base(expression, options, makeOptionWidget)
    {
        Expression = expression;
    }
    public override FilterWidget Clone()
    {
        return new OneToManyFilter<TOption>(Expression, Options, MakeOptionWidget);
    }

    private sealed class OneToManyExpression : NtMExpression
    {
        public override IEnumerable<GenOperator> SupportedOperators => [
            Operators.IsIn.Instance,
            Operators.IsNotIn.Instance
        ];
        public OneToManyExpression(Func<ThingAlike, TOption> lhs) : base(lhs)
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
