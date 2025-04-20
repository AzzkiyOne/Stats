using System;
using Stats.Widgets.Table.Filters.Operators.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table.Filters.Widgets;

public sealed class Widget_FilterInput_Bool
    : Widget,
      IWidget_FilterInput
{
    protected override Vector2 Size { get; set; }
    private readonly ThingMatcher<bool> _ThingMatcher;
    public IThingMatcher ThingMatcher => _ThingMatcher;
    public Widget_FilterInput_Bool(ThingMatcher<bool> thingMatcher)
    {
        _ThingMatcher = thingMatcher;
        Size = GetSize();
        thingMatcher.OnChange += HandleThingMatcherOnChange;
    }
    public Widget_FilterInput_Bool(Func<ThingRec, bool> valueFunc)
        : this(new ThingMatcher<bool>(valueFunc, true))
    {
    }
    private void HandleThingMatcherOnChange(IThingMatcher thingMathcer)
    {
        UpdateSize();
    }
    public override Vector2 GetSize()
    {
        if (_ThingMatcher.IsActive == false)
        {
            return Text.CalcSize(_ThingMatcher.Operator.ToString());
        }

        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    protected override void DrawContent(Rect rect)
    {
        switch (_ThingMatcher)
        {
            case { IsActive: false }:
                if (Verse.Widgets.ButtonTextSubtle(rect, _ThingMatcher.Operator.ToString()))
                {
                    _ThingMatcher.Set(true, Eq<bool>.Instance);
                }
                break;
            case { Value: true }:
                if (Verse.Widgets.ButtonImageFitted(rect, Verse.Widgets.CheckboxOnTex))
                {
                    _ThingMatcher.Value = false;
                }
                break;
            case { Value: false }:
                if (Verse.Widgets.ButtonImageFitted(rect, Verse.Widgets.CheckboxOffTex))
                {
                    _ThingMatcher.Reset();
                }
                break;
        }
    }
    public IWidget_FilterInput Clone()
    {
        return new Widget_FilterInput_Bool(_ThingMatcher);
    }
}
