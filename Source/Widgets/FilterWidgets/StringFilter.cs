using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets.FilterWidgets;

public sealed class StringFilter : FilterWidgetWithInputField<string, string>
{
    new private readonly StringExpression Expression;
    public StringFilter(Func<ThingAlike, string> lhs) : this(new StringExpression(lhs))
    {
    }
    private StringFilter(StringExpression expression) : base(expression)
    {
        Expression = expression;
    }
    protected override void DrawInputField(Rect rect)
    {
        Expression.Rhs = Verse.Widgets.TextField(rect, Expression.Rhs);
    }
    public override FilterWidget Clone()
    {
        return new StringFilter(Expression);
    }

    private sealed class StringExpression : GenExpression
    {
        public override IEnumerable<GenOperator> SupportedOperators => [
            Operators.Contains.Instance,
            Operators.NotContains.Instance,
        ];
        public StringExpression(Func<ThingAlike, string> lhs) : base(lhs, "")
        {
        }
        public override void Clear()
        {
            base.Clear();

            Rhs = "";
        }

        private static class Operators
        {
            public sealed class Contains : GenOperator
            {
                public Contains() { }
                public override bool Eval(string lhs, string rhs) =>
                    lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase);
                public override string ToString() => "~=";
                public static Contains Instance { get; } = new();
            }

            public sealed class NotContains : GenOperator
            {
                public NotContains() { }
                public override bool Eval(string lhs, string rhs) =>
                    lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase) == false;
                public override string ToString() => "!~=";
                public static NotContains Instance { get; } = new();
            }
        }
    }
}
