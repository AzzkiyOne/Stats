using System;
using UnityEngine;

namespace Stats.Widgets;

internal sealed class BooleanFilter<TObject> : FilterWidgetWithInputField<TObject, bool, bool>
{
    private Action<Rect> DrawValue;
    public BooleanFilter(Func<TObject, bool> lhs) : base(lhs, true, [Operators.IsEqualTo.Instance])
    {
        DrawValue = DrawTrue;
    }
    protected override void HandleStateChange(FilterWidget<TObject> _)
    {
        DrawValue = Rhs switch
        {
            true => DrawTrue,
            false => DrawFalse,
        };

        base.HandleStateChange(this);
    }
    private void DrawTrue(Rect rect)
    {
        if (Widgets.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOnTex))
        {
            Rhs = false;
        }
    }
    private void DrawFalse(Rect rect)
    {
        if (Widgets.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOffTex))
        {
            Rhs = true;
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
    public override void Reset()
    {
        base.Reset();

        Rhs = true;
    }

    private static class Operators
    {
        public sealed class IsEqualTo : AbsOperator
        {
            private IsEqualTo() : base("==") { }
            public override bool Eval(bool lhs, bool rhs) => lhs == rhs;
            public static IsEqualTo Instance { get; } = new();
        }
    }
}
