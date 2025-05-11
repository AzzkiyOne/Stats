using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stats.Widgets;

public sealed class BooleanFilter<TObject> : FilterWidgetWithInputField<TObject, bool, bool>
{
    new private readonly BooleanExpression Expression;
    private Action<Rect> DrawValue;
    public BooleanFilter(Func<TObject, bool> lhs) : this(new BooleanExpression(lhs))
    {
    }
    private BooleanFilter(BooleanExpression expression) : base(expression)
    {
        Expression = expression;
        DrawValue = DrawTrue;

        expression.OnChange += HandleStateChange;
    }
    private void HandleStateChange(AbsExpression _)
    {
        DrawValue = Expression switch
        {
            { Rhs: true } => DrawTrue,
            { Rhs: false } => DrawFalse,
        };

        Resize();
    }
    private void DrawTrue(Rect rect)
    {
        if (Widgets.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOnTex))
        {
            Expression.Rhs = false;
        }
    }
    private void DrawFalse(Rect rect)
    {
        if (Widgets.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOffTex))
        {
            Expression.Rhs = true;
        }
    }
    protected override Vector2 CalcInputFieldContentSize()
    {
        return new Vector2(Verse.Text.LineHeight, Verse.Text.LineHeight);
    }
    protected override void DrawInputField(Rect rect)
    {
        DrawValue(rect);
    }
    public override FilterWidget<TObject> Clone()
    {
        return new BooleanFilter<TObject>(Expression);
    }

    private sealed class BooleanExpression : GenExpression
    {
        public override IEnumerable<GenericOperator> SupportedOperators { get; } = [Operators.IsEqualTo.Instance];
        public BooleanExpression(Func<TObject, bool> lhs) : base(lhs, true)
        {
        }
        public override void Reset()
        {
            base.Reset();

            Rhs = true;
        }

        private static class Operators
        {
            public sealed class IsEqualTo : GenericOperator
            {
                private IsEqualTo() : base("==") { }
                public override bool Eval(bool lhs, bool rhs) => lhs == rhs;
                public static IsEqualTo Instance { get; } = new();
            }
        }
    }
}
