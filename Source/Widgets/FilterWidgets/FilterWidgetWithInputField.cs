using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public abstract class FilterWidgetWithInputField<TObject, TExprLhs, TExprRhs> : FilterWidget<TObject, TExprLhs, TExprRhs>
    where TExprRhs : notnull
{
    private Vector2 OperatorButtonSize;
    private Vector2 InputFieldSize;
    private const float MinInputFieldWidth = 36f;
    private readonly Vector2 ResetButtonSize;
    private bool ChildrenSizesAreCached = false;
    private static readonly TipSignal ResetButtonTooltip = "Reset";
    private readonly FloatMenu OperatorsMenu;
    private const float OperatorButtonPadding = Globals.GUI.Pad;
    protected FilterWidgetWithInputField(GenExpression expression) : base(expression)
    {
        ResetButtonSize = new Vector2(Text.LineHeight, Text.LineHeight);

        var operatorsMenuOptions = new List<FloatMenuOption>(expression.SupportedOperators.Count());

        foreach (var @operator in expression.SupportedOperators)
        {
            void handleOptionClick()
            {
                // TODO: This is a hack, but it'll do for now.
                if (expression.IsEmpty)
                {
                    FocusInputField();
                }

                expression.Operator = @operator;
            }
            var optionLabel = @operator.Description.Length > 0
                    ? $"{@operator.Symbol} - {@operator.Description}"
                    : @operator.Symbol;
            var option = new FloatMenuOption(optionLabel, handleOptionClick);

            operatorsMenuOptions.Add(option);
        }

        OperatorsMenu = new FloatMenu(operatorsMenuOptions);

        expression.OnChange += HandleStateChange;
    }
    protected sealed override Vector2 CalcSize()
    {
        if (ChildrenSizesAreCached == false)
        {
            OperatorButtonSize = CalcOperatorButtonSize(Expression);
            InputFieldSize = CalcInputFieldSize();
            ChildrenSizesAreCached = true;
        }

        var size = OperatorButtonSize;

        if (Expression.IsEmpty)
        {
            return size;
        }

        size.x += InputFieldSize.x + ResetButtonSize.x;
        size.y = Mathf.Max(size.y, InputFieldSize.y, ResetButtonSize.y);

        return size;
    }
    public sealed override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        if (Expression.IsEmpty || Expression.SupportedOperators.Count() > 1)
        {
            var operatorButtonRect = Expression.IsEmpty ? rect : rect.CutByX(OperatorButtonSize.x);

            if (DrawOperatorButton(operatorButtonRect, Expression))
            {
                if (Expression.SupportedOperators.Count() == 1)
                {
                    Expression.Operator = Expression.SupportedOperators.First();
                }
                else
                {
                    Find.WindowStack.Add(OperatorsMenu);
                }
            }
        }

        if (Expression.IsEmpty == false)
        {
            DrawInputField(rect with { width = rect.width - ResetButtonSize.x });

            var resetButtonRect = rect.RightPartPixels(ResetButtonSize.x);

            if (Widgets.Draw.ButtonImageSubtle(resetButtonRect, TexButton.CloseXSmall, 0.5f))
            {
                Expression.Reset();
            }

            if (Mouse.IsOver(resetButtonRect))
            {
                TooltipHandler.TipRegion(resetButtonRect, ResetButtonTooltip);
            }
        }
    }
    private static Vector2 CalcOperatorButtonSize(GenExpression expression)
    {
        var size = Text.CalcSize(expression.Operator.Symbol);
        size.x += OperatorButtonPadding * 2f;

        return size;
    }
    private static bool DrawOperatorButton(Rect rect, GenExpression expression)
    {
        if (expression.Operator.Description.Length > 0 && Mouse.IsOver(rect))
        {
            TooltipHandler.TipRegion(rect, expression.Operator.Description);
        }

        return Widgets.Draw.ButtonTextSubtle(rect, expression.Operator.Symbol, OperatorButtonPadding);
    }
    protected abstract Vector2 CalcInputFieldContentSize();
    private Vector2 CalcInputFieldSize()
    {
        var size = CalcInputFieldContentSize();
        size.x += Globals.GUI.EstimatedInputFieldInnerPadding * 2f;

        if (size.x < MinInputFieldWidth)
        {
            size.x = MinInputFieldWidth;
        }

        return size;
    }
    protected abstract void DrawInputField(Rect rect);
    private void HandleStateChange(AbsExpression _)
    {
        OperatorButtonSize = CalcOperatorButtonSize(Expression);
        InputFieldSize = CalcInputFieldSize();

        Resize();
    }
    protected virtual void FocusInputField()
    {
    }
}
