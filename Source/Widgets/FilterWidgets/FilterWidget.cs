using System.Collections.Generic;
using System.Linq;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public abstract class FilterWidget : WidgetBase
{
    public abstract FilterExpression FilterExpression { get; }
    public abstract FilterWidget Clone();
}

public abstract class FilterWidget<Lhs, Rhs> : FilterWidget
    where Rhs : notnull
{
    protected override Vector2 Size { get; set; }
    protected readonly FilterExpression<Lhs, Rhs> _FilterExpression;
    public override FilterExpression FilterExpression => _FilterExpression;
    protected FloatMenu OperatorsMenu { get; }
    public FilterWidget(
        FilterExpression<Lhs, Rhs> filterExpression,
        IEnumerable<RelationalOperator<Lhs, Rhs>> operators
    )
    {
        _FilterExpression = filterExpression;
        OperatorsMenu = MakeFloatMenu(filterExpression, operators);
        Size = GetSize();

        filterExpression.OnChange += HandleFilterExpressionChange;
    }
    private void HandleFilterExpressionChange(FilterExpression thingMathcer)
    {
        UpdateSize();
    }
    private static FloatMenu MakeFloatMenu(
        FilterExpression<Lhs, Rhs> filterExpression,
        IEnumerable<RelationalOperator<Lhs, Rhs>> operators
    )
    {
        var operatorsMenuOptions = new List<FloatMenuOption>(operators.Count());

        foreach (var @operator in operators)
        {
            var optionText = @operator.ToString();
            void optionAction() => filterExpression.Operator = @operator;
            var option = new FloatMenuOption(optionText, optionAction);

            operatorsMenuOptions.Add(option);
        }

        return new FloatMenu(operatorsMenuOptions);
    }
}
