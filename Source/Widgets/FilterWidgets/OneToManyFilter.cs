using System;
using System.Collections.Generic;

namespace Stats.Widgets;

public sealed class OneToManyFilter<TObject, TOption> : NToManyFilter<TObject, TOption, TOption>
{
    public OneToManyFilter(
        Func<TObject, TOption> lhs,
        IEnumerable<TOption> options,
        OptionWidgetFactory makeOptionWidget
    ) : base(lhs, options, makeOptionWidget, [
        Operators.IsIn.Instance,
        Operators.IsNotIn.Instance
    ])
    {
    }

    private static class Operators
    {
        public sealed class IsIn : AbsOperator
        {
            private IsIn() : base("∈", "Is one of selected") { }
            public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs);
            public static IsIn Instance { get; } = new();
        }

        // ∉
        public sealed class IsNotIn : AbsOperator
        {
            private IsNotIn() : base("!∈", "Is not one of selected") { }
            public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs) == false;
            public static IsNotIn Instance { get; } = new();
        }
    }
}
