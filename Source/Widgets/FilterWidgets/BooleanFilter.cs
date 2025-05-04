using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class BooleanFilter<T> : FilterWidget<T, bool, bool>
{
    new private readonly BooleanExpression Expression;
    private Action<Rect> DrawValue;
    public BooleanFilter(Func<T, bool> lhs) : this(new BooleanExpression(lhs))
    {
    }
    private BooleanFilter(BooleanExpression expression) : base(expression)
    {
        Expression = expression;
        DrawValue = DrawEmpty;

        expression.OnChange += HandleStateChange;
    }
    private void HandleStateChange(AbsExpression _)
    {
        DrawValue = Expression switch
        {
            { IsEmpty: true } => DrawEmpty,
            { Rhs: true } => DrawTrue,
            { Rhs: false } => DrawFalse,
        };

        Resize();
    }
    protected override Vector2 CalcSize()
    {
        if (base.Expression.IsEmpty)
        {
            return Text.CalcSize(Expression.Operator.ToString());
        }

        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        DrawValue(rect);
    }
    private void DrawEmpty(Rect rect)
    {
        if (Widgets.Draw.ButtonTextSubtle(rect, Expression.Operator.ToString()))
        {
            Expression.Operator = Expression.SupportedOperators.First();
        }
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
            Expression.Clear();
        }
    }
    public override FilterWidget<T> Clone()
    {
        return new BooleanFilter<T>(Expression);
    }

    private sealed class BooleanExpression : GenExpression
    {
        public override IEnumerable<GenOperator> SupportedOperators => [Operators.EqualTo.Instance];
        public BooleanExpression(Func<T, bool> lhs) : base(lhs, true)
        {
        }
        public override void Clear()
        {
            base.Clear();

            Rhs = true;
        }

        private static class Operators
        {
            public sealed class EqualTo : GenOperator
            {
                private EqualTo() { }
                public override bool Eval(bool lhs, bool rhs) => lhs == rhs;
                public override string ToString() => "==";
                public static EqualTo Instance { get; } = new();
            }
        }
    }
}
