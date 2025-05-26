using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal abstract class FilterWidgetWithInputField<TObject, TExprLhs, TExprRhs> : FilterWidget<TObject, TExprLhs, TExprRhs>
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
    private readonly IEnumerable<AbsOperator> Operators;
    protected FilterWidgetWithInputField(
        Func<TObject, TExprLhs> lhs,
        TExprRhs rhs,
        IEnumerable<AbsOperator> operators
    ) : base(lhs, rhs)
    {
        Operators = operators;
        ResetButtonSize = new Vector2(Text.LineHeight, Text.LineHeight);

        var operatorsMenuOptions = new List<FloatMenuOption>(operators.Count());

        foreach (var @operator in operators)
        {
            void handleOptionClick()
            {
                // TODO: This is a hack, but it'll do for now.
                if (IsActive == false)
                {
                    FocusInputField();
                }

                Operator = @operator;
            }
            var operatorString = @operator.Symbol.Colorize(Globals.GUI.ActiveFilterOperatorColor);
            var optionLabel = @operator.Description.Length > 0
                    ? $"{operatorString} - {@operator.Description}"
                    : operatorString;
            var option = new FloatMenuOption(optionLabel, handleOptionClick);

            operatorsMenuOptions.Add(option);
        }

        OperatorsMenu = new FloatMenu(operatorsMenuOptions);

        OnChange += HandleStateChange;
    }
    protected sealed override Vector2 CalcSize()
    {
        if (ChildrenSizesAreCached == false)
        {
            OperatorButtonSize = CalcOperatorButtonSize();
            InputFieldSize = CalcInputFieldSize();
            ChildrenSizesAreCached = true;
        }

        var size = OperatorButtonSize;

        if (IsActive)
        {
            size.x += InputFieldSize.x + ResetButtonSize.x;
            size.y = Mathf.Max(size.y, InputFieldSize.y, ResetButtonSize.y);
        }

        return size;
    }
    public sealed override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        if (IsActive)
        {
            if (Operators.Count() > 1)
            {
                var operatorButtonRect = rect.CutByX(OperatorButtonSize.x);

                if (DrawOperatorButton(operatorButtonRect))
                {
                    Find.WindowStack.Add(OperatorsMenu);
                }
            }

            DrawInputField(rect with { width = rect.width - ResetButtonSize.x });

            var resetButtonRect = rect.RightPartPixels(ResetButtonSize.x);

            if (Widgets.Draw.ButtonImageSubtle(resetButtonRect, TexButton.CloseXSmall, 0.5f))
            {
                Reset();
            }

            if (Mouse.IsOver(resetButtonRect))
            {
                TooltipHandler.TipRegion(resetButtonRect, ResetButtonTooltip);
            }
        }
        else
        {
            if (DrawOperatorButton(rect))
            {
                if (Operators.Count() == 1)
                {
                    Operator = Operators.First();
                }
                else
                {
                    Find.WindowStack.Add(OperatorsMenu);
                }
            }
        }
    }
    private Vector2 CalcOperatorButtonSize()
    {
        if (IsActive == false || Operators.Count() > 1)
        {
            var size = Text.CalcSize(Operator.Symbol);
            size.x += OperatorButtonPadding * 2f;

            return size;
        }

        return Vector2.zero;
    }
    private bool DrawOperatorButton(Rect rect)
    {
        if (Operator.Description.Length > 0 && Mouse.IsOver(rect))
        {
            TooltipHandler.TipRegion(rect, Operator.Description);
        }

        var color = IsActive ? Globals.GUI.ActiveFilterOperatorColor : Color.white;

        return Widgets.Draw.ButtonTextSubtle(rect, Operator.Symbol, color, OperatorButtonPadding);
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
    protected virtual void HandleStateChange(FilterWidget<TObject> _)
    {
        OperatorButtonSize = CalcOperatorButtonSize();
        InputFieldSize = CalcInputFieldSize();

        Resize();
    }
    protected virtual void FocusInputField()
    {
    }
}
