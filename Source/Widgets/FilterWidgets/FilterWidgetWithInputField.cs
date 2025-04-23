using System.Collections.Generic;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public abstract class FilterWidgetWithInputField<Lhs, Rhs> : FilterWidget<Lhs, Rhs>
    where Rhs : notnull, Lhs
{
    public FilterWidgetWithInputField(
        FilterExpression<Lhs, Rhs> filterExpression,
        IEnumerable<RelationalOperator<Lhs, Rhs>> operators
    ) : base(filterExpression, operators)
    {
    }
    public override Vector2 GetSize()
    {
        if (_FilterExpression.IsEmpty)
        {
            return Text.CalcSize(_FilterExpression.Operator.ToString());
        }

        var size = Text.CalcSize(
            _FilterExpression.Operator.ToString() + _FilterExpression.Value
        );
        size.x += Globals.UI.EstimatedInputFieldInnerPadding * 2f;

        return size;
    }
    protected override void DrawContent(Rect rect)
    {
        if
        (
            Verse.Widgets.ButtonTextSubtle(
                _FilterExpression.IsEmpty
                    ? rect
                    : rect.CutByX(rect.height),
                _FilterExpression.Operator.ToString()
            )
        )
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (_FilterExpression.IsEmpty == false)
        {
            DrawInputField(rect);
        }
    }
    protected abstract void DrawInputField(Rect rect);
}
