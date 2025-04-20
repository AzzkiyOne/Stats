using System;
using Stats.Widgets.Table.Filters.Operators.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table.Filters.Widgets;

public sealed class BooleanFilterWidget
    : Widget,
      IFilterWidget
{
    protected override Vector2 Size { get; set; }
    private readonly FilterExpression<bool> _FilterExpression;
    public IFilterExpression FilterExpression => _FilterExpression;
    public BooleanFilterWidget(FilterExpression<bool> filterExpression)
    {
        _FilterExpression = filterExpression;
        Size = GetSize();
        filterExpression.OnChange += HandleThingMatcherChange;
    }
    public BooleanFilterWidget(Func<ThingAlike, bool> valueFunc)
        : this(new FilterExpression<bool>(valueFunc, true, Any<bool>.Instance, Any<bool>.Instance))
    {
    }
    private void HandleThingMatcherChange(IFilterExpression _)
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
    public IFilterWidget Clone()
    {
        return new BooleanFilterWidget(_FilterExpression);
    }
}
