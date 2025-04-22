using System.Collections.Generic;
using System.Linq;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public abstract class FilterWidget<T>
    : Widget,
      IFilterWidget
    where T : notnull
{
    protected override Vector2 Size { get; set; }
    protected readonly FilterExpression<T> _FilterExpression;
    public IFilterExpression FilterExpression => _FilterExpression;
    protected FloatMenu OperatorsMenu { get; }
    public FilterWidget(
        FilterExpression<T> filterExpression,
        IEnumerable<IRelationalOperator<T>> operators
    )
    {
        _FilterExpression = filterExpression;
        OperatorsMenu = MakeFloatMenu(filterExpression, operators);
        Size = GetSize();

        filterExpression.OnChange += HandleFilterExpressionChange;
    }
    private void HandleFilterExpressionChange(IFilterExpression thingMathcer)
    {
        UpdateSize();
    }
    public abstract IFilterWidget Clone();
    private static FloatMenu MakeFloatMenu(
        FilterExpression<T> filterExpression,
        IEnumerable<IRelationalOperator<T>> operators
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
