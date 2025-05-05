using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public abstract class FilterWidgetWithInputField<TObject, TExprLhs, TExprRhs> : FilterWidget<TObject, TExprLhs, TExprRhs>
    where TExprRhs : notnull
{
    private Vector2 OperatorButtonSize;
    private Vector2 InputFieldSize;
    private readonly FloatMenu OperatorsMenu;
    private const float OperatorButtonPadding = Globals.GUI.Pad;
    protected FilterWidgetWithInputField(GenExpression expression) : base(expression)
    {
        OperatorButtonSize = CalcOperatorButtonSize(expression);
        InputFieldSize = CalcInputFieldSize(expression);

        var operatorsMenuOptions = new List<FloatMenuOption>(expression.SupportedOperators.Count() + 1)
        {
            new FloatMenuOption("Clear", expression.Clear),
        };

        foreach (var @operator in expression.SupportedOperators)
        {
            var option = new FloatMenuOption(@operator.ToString(), () => expression.Operator = @operator);

            operatorsMenuOptions.Add(option);
        }

        OperatorsMenu = new FloatMenu(operatorsMenuOptions);

        expression.OnChange += HandleStateChange;
    }
    protected sealed override Vector2 CalcSize()
    {
        var size = OperatorButtonSize;

        if (Expression.IsEmpty)
        {
            return size;
        }

        size.x += InputFieldSize.x;
        size.y = Mathf.Max(size.y, InputFieldSize.y);

        return size;
    }
    public sealed override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        var operatorButtonRect = Expression.IsEmpty ? rect : rect.CutByX(OperatorButtonSize.x);

        if (DrawOperatorButton(operatorButtonRect, Expression))
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (Expression.IsEmpty == false)
        {
            DrawInputField(rect);
        }
    }
    private static Vector2 CalcOperatorButtonSize(GenExpression expression)
    {
        var size = Text.CalcSize(expression.OperatorString);
        size.x += OperatorButtonPadding * 2f;

        return size;
    }
    private static bool DrawOperatorButton(Rect rect, GenExpression expression)
    {
        return Widgets.Draw.ButtonTextSubtle(rect, expression.OperatorString, OperatorButtonPadding);
    }
    private static Vector2 CalcInputFieldSize(GenExpression expression)
    {
        var size = Text.CalcSize(expression.RhsString);
        size.x += Globals.GUI.EstimatedInputFieldInnerPadding * 2f;

        return size;
    }
    protected abstract void DrawInputField(Rect rect);
    private void HandleStateChange(AbsExpression _)
    {
        OperatorButtonSize = CalcOperatorButtonSize(Expression);
        InputFieldSize = CalcInputFieldSize(Expression);

        Resize();
    }
}
