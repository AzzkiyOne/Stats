﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets;

public sealed class StringFilter<TObject> : FilterWidgetWithInputField<TObject, string, string>
{
    new private readonly StringExpression Expression;
    public StringFilter(Func<TObject, string> lhs) : this(new StringExpression(lhs))
    {
    }
    private StringFilter(StringExpression expression) : base(expression)
    {
        Expression = expression;
    }
    protected override Vector2 CalcInputFieldContentSize()
    {
        return Verse.Text.CalcSize(Expression.Rhs);
    }
    protected override void DrawInputField(Rect rect)
    {
        Expression.Rhs = Verse.Widgets.TextField(rect, Expression.Rhs);
    }
    public override FilterWidget<TObject> Clone()
    {
        return new StringFilter<TObject>(Expression);
    }

    private sealed class StringExpression : GenExpression
    {
        public override IEnumerable<GenericOperator> SupportedOperators { get; } = [
            Operators.Contains.Instance,
            Operators.NotContains.Instance,
        ];
        public StringExpression(Func<TObject, string> lhs) : base(lhs, "")
        {
        }
        public override void Reset()
        {
            base.Reset();

            Rhs = "";
        }

        private static class Operators
        {
            public sealed class Contains : GenericOperator
            {
                public Contains() : base("~=", "Contains") { }
                public override bool Eval(string lhs, string rhs) =>
                    lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase);
                public static Contains Instance { get; } = new();
            }

            public sealed class NotContains : GenericOperator
            {
                public NotContains() : base("!~=", "Does not contains") { }
                public override bool Eval(string lhs, string rhs) =>
                    lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase) == false;
                public static NotContains Instance { get; } = new();
            }
        }
    }
}
