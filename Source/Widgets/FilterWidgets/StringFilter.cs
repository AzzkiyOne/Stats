using System;
using System.Collections.Generic;
using Stats.RelationalOperators;
using Stats.RelationalOperators.String;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class StringFilter : FilterWidget<StringFilterState>
{
    private readonly StringFilterState SharedState;
    public override FilterExpression State => SharedState;
    protected override FloatMenu OperatorsMenu { get; }
    private StringFilter(StringFilterState state) : base(state)
    {
        SharedState = state;

        RelationalOperator<string, string>[] operators =
        [
            Contains.Instance,
            NotContains.Instance,
        ];

        var operatorsMenuOptions = new List<FloatMenuOption>(operators.Length + 1)
        {
            MakeClearStateOperatorsMenuOption(state),
        };

        foreach (var @operator in operators)
        {
            var option = new FloatMenuOption(@operator.ToString(), () => state.Operator = @operator);

            operatorsMenuOptions.Add(option);
        }

        OperatorsMenu = new FloatMenu(operatorsMenuOptions);
    }
    public StringFilter(Func<ThingAlike, string> lhs) : this(new StringFilterState(lhs))
    {
    }
    protected override void DrawInputField(Rect rect)
    {
        SharedState.Rhs = Verse.Widgets.TextField(rect, SharedState.Rhs);
    }
    public override FilterWidget Clone()
    {
        return new StringFilter(SharedState);
    }
}

public sealed class StringFilterState : FilterExpression<string, string>
{
    public StringFilterState(Func<ThingAlike, string> lhs) : base(lhs, "")
    {
    }
    public override void Clear()
    {
        base.Clear();

        Rhs = "";
    }
}
