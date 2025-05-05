using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class ManyToManyFilter<TObject, TOption> : NToManyFilter<TObject, HashSet<TOption>, TOption>
{
    new private readonly ManyToManyExpression Expression;
    public ManyToManyFilter(
        Func<TObject, HashSet<TOption>> lhs,
        IEnumerable<TOption> options,
        OptionWidgetFactory makeOptionWidget
    ) : this(new ManyToManyExpression(lhs), options, makeOptionWidget)
    {
    }
    private ManyToManyFilter(
        ManyToManyExpression expression,
        IEnumerable<TOption> options,
        OptionWidgetFactory makeOptionWidget
    ) : base(expression, options, makeOptionWidget)
    {
        Expression = expression;
    }
    public override FilterWidget<TObject> Clone()
    {
        return new ManyToManyFilter<TObject, TOption>(Expression, Options, MakeOptionWidget);
    }

    private sealed class ManyToManyExpression : NtMExpression
    {
        public override IEnumerable<GenOperator> SupportedOperators => [
            Operators.IntersectsWith.Instance,
            Operators.IsSubsetOf.Instance,
            Operators.IsNotSubsetOf.Instance,
            Operators.IsSupersetOf.Instance,
            Operators.IsNotSupersetOf.Instance,
        ];
        public ManyToManyExpression(Func<TObject, HashSet<TOption>> lhs) : base(lhs)
        {
        }

        private static class Operators
        {
            public sealed class IntersectsWith : GenOperator
            {
                private IntersectsWith() { }
                public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains);
                public override string ToString() => "∩";
                public static IntersectsWith Instance { get; } = new();
            }

            public sealed class IsSupersetOf : GenOperator
            {
                private IsSupersetOf() { }
                public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSupersetOf(rhs);
                public override string ToString() => "⊇";
                public static IsSupersetOf Instance { get; } = new();
            }

            public sealed class IsNotSupersetOf : GenOperator
            {
                private IsNotSupersetOf() { }
                public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSupersetOf(rhs) == false;
                public override string ToString() => "⊅";
                public static IsNotSupersetOf Instance { get; } = new();
            }

            public sealed class IsSubsetOf : GenOperator
            {
                private IsSubsetOf() { }
                public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSubsetOf(rhs);
                public override string ToString() => "⊆";
                public static IsSubsetOf Instance { get; } = new();
            }

            public sealed class IsNotSubsetOf : GenOperator
            {
                private IsNotSubsetOf() { }
                public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSubsetOf(rhs) == false;
                public override string ToString() => "⊄";
                public static IsNotSubsetOf Instance { get; } = new();
            }
        }
    }
}
