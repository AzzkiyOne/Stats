using System;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class BooleanFilter
    : FilterWidget
{
    protected override Vector2 Size { get; set; }
    private readonly FilterExpression<bool> _FilterExpression;
    public override FilterExpression FilterExpression => _FilterExpression;
    public BooleanFilter(FilterExpression<bool> filterExpression)
    {
        _FilterExpression = filterExpression;
        Size = GetSize();
        filterExpression.OnChange += HandleThingMatcherChange;
    }
    public BooleanFilter(Func<ThingAlike, bool> valueFunc)
        : this(new FilterExpression<bool>(valueFunc, true, Any<bool>.Instance, Any<bool>.Instance))
    {
    }
    private void HandleThingMatcherChange(FilterExpression _)
    {
        UpdateSize();
    }
    public override Vector2 GetSize()
    {
        if (_FilterExpression.IsActive == false)
        {
            return Text.CalcSize(_FilterExpression.Operator.ToString());
        }

        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    protected override void DrawContent(Rect rect)
    {
        switch (_FilterExpression)
        {
            case { IsActive: false }:
                if (Verse.Widgets.ButtonTextSubtle(rect, _FilterExpression.Operator.ToString()))
                {
                    _FilterExpression.Set(true, Eq<bool>.Instance);
                }
                break;
            case { Value: true }:
                if (Verse.Widgets.ButtonImageFitted(rect, Verse.Widgets.CheckboxOnTex))
                {
                    _FilterExpression.Value = false;
                }
                break;
            case { Value: false }:
                if (Verse.Widgets.ButtonImageFitted(rect, Verse.Widgets.CheckboxOffTex))
                {
                    _FilterExpression.Reset();
                }
                break;
        }
    }
    public override FilterWidget Clone()
    {
        return new BooleanFilter(_FilterExpression);
    }
}
