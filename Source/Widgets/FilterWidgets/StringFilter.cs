using System;
using System.Collections.Generic;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class StringFilter
    : FilterWidgetWithInputField<string>
{
    private static readonly RelationalOperator<string>[] DefaultOperators =
        [
            Any<string>.Instance,
            Contains.Instance,
            ContainsNot.Instance,
        ];
    private const string Description = "Use \",\" to search by multiple terms.";
    public StringFilter(
        FilterExpression<string> filterExpression,
        IEnumerable<RelationalOperator<string>> operators
    ) : base(filterExpression, operators)
    {
    }
    public StringFilter(Func<ThingAlike, string> valueFunc)
        : this(
            new FilterExpression<string>(
                valueFunc,
                "",
                Any<string>.Instance,
                Any<string>.Instance
            ),
            DefaultOperators
        )
    {
    }
    protected override void DrawInputField(Rect rect)
    {
        _FilterExpression.Value = Verse.Widgets.TextField(rect, _FilterExpression.Value);
        TooltipHandler.TipRegion(rect, Description);
    }
    public override FilterWidget Clone()
    {
        return new StringFilter(_FilterExpression, DefaultOperators);
    }
}
