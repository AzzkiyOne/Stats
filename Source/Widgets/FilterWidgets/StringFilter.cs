using System;
using System.Collections.Generic;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class StringFilter : FilterWidget<string, string>
{
    private static readonly RelationalOperator<string, string>[] DefaultOperators =
        [
            ContainsAnyOf.Instance,
            NotContainsAnyOf.Instance,
        ];
    private const string Description = "Use \",\" to search by multiple terms.";
    private StringFilter(
        FilterExpression<string, string> value,
        IEnumerable<RelationalOperator<string, string>> operators
    ) : base(value, operators)
    {
    }
    public StringFilter(Func<ThingAlike, string> lhs)
        : this(new FilterExpression<string, string>(lhs, ""), DefaultOperators)
    {
    }
    protected override void DrawInputField(Rect rect)
    {
        _Value.Rhs = Verse.Widgets.TextField(rect, _Value.Rhs);
        TooltipHandler.TipRegion(rect, Description);
    }
    public override FilterWidget Clone()
    {
        return new StringFilter(_Value, DefaultOperators);
    }
}
