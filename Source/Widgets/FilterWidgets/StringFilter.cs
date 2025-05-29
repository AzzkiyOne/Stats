using System;
using UnityEngine;

namespace Stats.Widgets;

internal sealed class StringFilter<TObject> : FilterWidgetWithInputField<TObject, string, string>
{
    public StringFilter(Func<TObject, string> lhs) : base(
        lhs,
        "",
        [
            Operators.Contains.Instance,
            Operators.NotContains.Instance,
        ],
        Operators.Contains.Instance
    )
    {
    }
    protected override Vector2 CalcInputFieldContentSize()
    {
        return Verse.Text.CalcSize(Rhs);
    }
    protected override void DrawInputField(Rect rect)
    {
        Rhs = Verse.Widgets.TextField(rect, Rhs);
    }
    public override void Reset()
    {
        base.Reset();

        Rhs = "";
    }

    private static class Operators
    {
        public sealed class Contains : AbsOperator
        {
            public Contains() : base("~=", "Contains") { }
            public override bool Eval(string lhs, string rhs) =>
                lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase);
            public static Contains Instance { get; } = new();
        }

        public sealed class NotContains : AbsOperator
        {
            public NotContains() : base("!~=", "Does not contains") { }
            public override bool Eval(string lhs, string rhs) =>
                lhs.Contains(rhs, StringComparison.CurrentCultureIgnoreCase) == false;
            public static NotContains Instance { get; } = new();
        }
    }
}
