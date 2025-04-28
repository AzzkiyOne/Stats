using System.Collections.Generic;
using System.Linq;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public abstract class FilterWidget : Widget
{
    public abstract FilterExpression Value { get; }
    public abstract FilterWidget Clone();
}

// TODO: TLhs and TRhs are not very descriptive names here. I even think they leak abstraction.
public abstract class FilterWidget<TLhs, TRhs> : FilterWidget where TRhs : notnull
{
    private Vector2 OperatorButtonSize;
    private Vector2 InputFieldSize;
    protected readonly FilterExpression<TLhs, TRhs> _Value;
    public override FilterExpression Value => _Value;
    protected FloatMenu OperatorsMenu { get; }
    private const float OperatorButtonPadding = Globals.GUI.Pad;
    protected FilterWidget(
        FilterExpression<TLhs, TRhs> value,
        IEnumerable<RelationalOperator<TLhs, TRhs>> operators
    )
    {
        _Value = value;
        OperatorsMenu = MakeOperatorsMenu(value, operators);
        OperatorButtonSize = CalcOperatorButtonSize();
        InputFieldSize = CalcInputFieldSize();

        Resize();

        value.OnChange += HandleValueChange;
    }
    protected override Vector2 CalcSize()
    {
        if (_Value.IsEmpty)
        {
            return OperatorButtonSize;
        }

        var size = OperatorButtonSize;
        size.x += InputFieldSize.x;
        size.y = Mathf.Max(size.y, InputFieldSize.y);

        return size;
    }
    private Vector2 CalcOperatorButtonSize()
    {
        var size = Text.CalcSize(_Value.Operator.ToString());
        size.x += OperatorButtonPadding * 2f;

        return size;
    }
    protected virtual Vector2 CalcInputFieldSize()
    {
        var size = Text.CalcSize(_Value.Rhs.ToString());
        size.x += Globals.GUI.EstimatedInputFieldInnerPadding * 2f;

        return size;
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        if
        (
            Verse.Widgets.ButtonTextSubtle(
                _Value.IsEmpty ? rect : rect.CutByX(OperatorButtonSize.x),
                _Value.Operator.ToString(),
                textLeftMargin: OperatorButtonPadding
            )
        )
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (_Value.IsEmpty == false)
        {
            DrawInputField(rect);
        }
    }
    protected abstract void DrawInputField(Rect rect);
    protected virtual void HandleValueChange(FilterExpression value)
    {
        OperatorButtonSize = CalcOperatorButtonSize();
        InputFieldSize = CalcInputFieldSize();

        Resize();
    }
    private static FloatMenu MakeOperatorsMenu(
        FilterExpression<TLhs, TRhs> value,
        IEnumerable<RelationalOperator<TLhs, TRhs>> operators
    )
    {
        var operatorsMenuOptions = new List<FloatMenuOption>(operators.Count() + 1)
        {
            MakeOperatorsMenuOption(
                FilterExpression<TLhs, TRhs>.EmptyOperator.Instance,
                value
            )
        };

        foreach (var @operator in operators)
        {
            var option = MakeOperatorsMenuOption(@operator, value);

            operatorsMenuOptions.Add(option);
        }

        return new FloatMenu(operatorsMenuOptions);
    }
    private static FloatMenuOption MakeOperatorsMenuOption(
        RelationalOperator<TLhs, TRhs> @operator,
        FilterExpression<TLhs, TRhs> value
    )
    {
        return new FloatMenuOption(
            @operator.ToString(),
            () => value.Operator = @operator
        );
    }
}
