using System;
using System.Collections.Generic;
using Stats.Widgets.Table.Filters.Operators.Generic;
using Stats.Widgets.Table.Filters.Operators.String;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table.Filters.Widgets;

public sealed class Widget_FilterInput_Str
    : Widget,
      IWidget_FilterInput
{
    protected override Vector2 Size { get; set; }
    private readonly ThingMatcher<string> _ThingMatcher;
    public IThingMatcher ThingMatcher => _ThingMatcher;
    private readonly FloatMenu OperatorsMenu;
    private const string Description = "Use \",\" to search by multiple terms.";
    public Widget_FilterInput_Str(ThingMatcher<string> thingMatcher)
    {
        _ThingMatcher = thingMatcher;
        IRelationalOperator<string>[] operators = [
            Any<string>.Instance,
            Contains.Instance,
            ContainsNot.Instance,
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
    public Widget_FilterInput_Str(Func<ThingRec, string> valueFunc)
        : this(new ThingMatcher<string>(valueFunc, ""))
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
            _ThingMatcher.Operator.ToString() + _ThingMatcher.Value
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
            _ThingMatcher.Value = Verse.Widgets.TextField(rect, _ThingMatcher.Value);
            TooltipHandler.TipRegion(rect, Description);
        }
    }
    public IWidget_FilterInput Clone()
    {
        return new Widget_FilterInput_Str(_ThingMatcher);
    }
}
