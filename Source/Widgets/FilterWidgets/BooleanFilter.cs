using System;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class BooleanFilter : FilterWidget
{
    protected override Vector2 Size { get; set; }
    private readonly FilterExpression<bool, bool> _Value;
    public override FilterExpression Value => _Value;
    private Action<Rect> DrawValue;
    public BooleanFilter(FilterExpression<bool, bool> value)
    {
        _Value = value;
        Size = GetSize();
        DrawValue = DrawEmpty;

        value.OnChange += HandleValueChange;
    }
    public BooleanFilter(Func<ThingAlike, bool> lhs)
        : this(new FilterExpression<bool, bool>(lhs, true))
    {
    }
    private void HandleValueChange(FilterExpression _)
    {
        DrawValue = _Value switch
        {
            { IsEmpty: true } => DrawEmpty,
            { Rhs: true } => DrawTrue,
            { Rhs: false } => DrawFalse,
        };

        UpdateSize();
    }
    public override Vector2 GetSize()
    {
        if (_Value.IsEmpty)
        {
            return Text.CalcSize(_Value.Operator.ToString());
        }

        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    protected override void DrawContent(Rect rect) => DrawValue(rect);
    private void DrawEmpty(Rect rect)
    {
        if (Verse.Widgets.ButtonTextSubtle(rect, _Value.Operator.ToString()))
        {
            _Value.Set(true, EqualTo<bool, bool>.Instance);
        }
    }
    private void DrawTrue(Rect rect)
    {
        if (Verse.Widgets.ButtonImageFitted(rect, Verse.Widgets.CheckboxOnTex))
        {
            _Value.Rhs = false;
        }
    }
    private void DrawFalse(Rect rect)
    {
        if (Verse.Widgets.ButtonImageFitted(rect, Verse.Widgets.CheckboxOffTex))
        {
            _Value.Clear();
        }
    }
    public override FilterWidget Clone()
    {
        return new BooleanFilter(_Value);
    }
}
