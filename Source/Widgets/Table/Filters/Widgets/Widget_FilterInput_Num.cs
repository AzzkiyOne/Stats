using System;
using System.Collections.Generic;
using Stats.Widgets.Table.Filters.Operators.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table.Filters.Widgets;

public sealed class Widget_FilterInput_Num
    : Widget,
      IWidget_FilterInput
{
    protected override Vector2 Size { get; set; }
    private readonly ThingMatcher<float> _ThingMatcher;
    public IThingMatcher ThingMatcher => _ThingMatcher;
    private readonly FloatMenu OperatorsMenu;
    private string ValueStrBuffer = "";
    public Widget_FilterInput_Num(ThingMatcher<float> thingMatcher)
    {
        _ThingMatcher = thingMatcher;
        IRelationalOperator<float>[] operators = [
            Any<float>.Instance,
            Eq<float>.Instance,
            EqNot<float>.Instance,
            Gt<float>.Instance,
            Lt<float>.Instance,
            GtOrEq<float>.Instance,
            LtOrEq<float>.Instance,
        ];
        var menuOptions = new List<FloatMenuOption>(operators.Length);
        foreach (var op in operators)
        {
            var optText = op.ToString();
            void optCb() => _ThingMatcher.Operator = op;
            menuOptions.Add(new FloatMenuOption(optText, optCb));
        }
        OperatorsMenu = new FloatMenu(menuOptions);
        Size = GetSize();
        thingMatcher.OnChange += HandleThingMatcherOnChange;
    }
    public Widget_FilterInput_Num(Func<ThingRec, float> valueFunc)
        : this(new ThingMatcher<float>(valueFunc, 0f))
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

        var size = Text.CalcSize(
            _ThingMatcher.Operator.ToString() + ValueStrBuffer
        );
        size.x += Constants.EstimatedInputFieldInnerPadding * 2;

        return size;
    }
    protected override void DrawContent(Rect rect)
    {
        if
        (
            Verse.Widgets.ButtonTextSubtle(
                _ThingMatcher.IsActive == false
                    ? rect
                    : rect.CutByX(rect.height),
                _ThingMatcher.Operator.ToString()
            )
        )
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (_ThingMatcher.IsActive)
        {
            var num = _ThingMatcher.Value;
            Verse.Widgets.TextFieldNumeric(rect, ref num, ref ValueStrBuffer);
            _ThingMatcher.Value = num;
        }
    }
    public IWidget_FilterInput Clone()
    {
        return new Widget_FilterInput_Num(_ThingMatcher);
    }
}
