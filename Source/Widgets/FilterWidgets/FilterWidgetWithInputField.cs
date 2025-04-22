using System.Collections.Generic;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public abstract class FilterWidgetWithInputField<T>
    : FilterWidget<T>
    where T : notnull
{
    public FilterWidgetWithInputField(
        FilterExpression<T> filterExpression,
        IEnumerable<IRelationalOperator<T>> operators
    ) : base(filterExpression, operators)
    {
    }
    public override Vector2 GetSize()
    {
        if (_FilterExpression.IsActive == false)
        {
            return Text.CalcSize(_FilterExpression.Operator.ToString());
        }

        var size = Text.CalcSize(
            _FilterExpression.Operator.ToString() + _FilterExpression.Value
        );
        size.x += Constants.EstimatedInputFieldInnerPadding * 2f;

        return size;
    }
    protected override void DrawContent(Rect rect)
    {
        if
        (
            Verse.Widgets.ButtonTextSubtle(
                _FilterExpression.IsActive == false
                    ? rect
                    : rect.CutByX(rect.height),
                _FilterExpression.Operator.ToString()
            )
        )
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (_FilterExpression.IsActive)
        {
            DrawInputField(rect);
        }
    }
    protected abstract void DrawInputField(Rect rect);
}
