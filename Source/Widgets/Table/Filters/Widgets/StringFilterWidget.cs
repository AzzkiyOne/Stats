using System;
using System.Collections.Generic;
using Stats.Widgets.Table.Filters.Operators;
using Stats.Widgets.Table.Filters.Operators.Generic;
using Stats.Widgets.Table.Filters.Operators.String;
using UnityEngine;
using Verse;

namespace Stats.Widgets.Table.Filters.Widgets;

public sealed class StringFilterWidget
    : FilterWidgetWithInputField<string>
{
    private static readonly IRelationalOperator<string>[] DefaultOperators =
        [
            Any<string>.Instance,
            Contains.Instance,
            ContainsNot.Instance,
        ];
    private const string Description = "Use \",\" to search by multiple terms.";
    public StringFilterWidget(
        FilterExpression<string> filterExpression,
        IEnumerable<IRelationalOperator<string>> operators
    ) : base(filterExpression, operators)
    {
    }
    public StringFilterWidget(Func<ThingAlike, string> valueFunc)
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
    public override IFilterWidget Clone()
    {
        return new StringFilterWidget(_FilterExpression, DefaultOperators);
    }
}
