using System;
using System.Collections.Generic;

namespace Stats.Widgets;

internal sealed class OTMFilter<TObject, TOption> : NTMFilter<TObject, TOption, TOption>
{
    public OTMFilter(
        Func<TObject, TOption> lhs,
        IEnumerable<NTMFilterOption<TOption>> options
    ) : base(
        lhs,
        options,
        [
            Operators.IsIn.Instance,
            Operators.IsNotIn.Instance
        ],
        Operators.IsIn.Instance
    )
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
