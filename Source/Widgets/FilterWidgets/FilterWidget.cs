using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public abstract class FilterWidget : Widget
{
    public abstract FilterExpression State { get; }
    public abstract FilterWidget Clone();
}

public abstract class FilterWidget<TState> : FilterWidget where TState : FilterExpression
{
    private Vector2 OperatorButtonSize;
    private Vector2 InputFieldSize;
    protected abstract FloatMenu OperatorsMenu { get; }
    private const float OperatorButtonPadding = Globals.GUI.Pad;
    protected FilterWidget(FilterExpression state)
    {
        OperatorButtonSize = CalcOperatorButtonSize(state);
        InputFieldSize = CalcInputFieldSize(state);

        state.OnChange += HandleStateChange;
    }
    protected override Vector2 CalcSize()
    {
        var size = OperatorButtonSize;

        if (State.IsEmpty)
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

        var operatorButtonRect = State.IsEmpty ? rect : rect.CutByX(OperatorButtonSize.x);

        if (DrawOperatorButton(operatorButtonRect, State))
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (State.IsEmpty == false)
        {
            DrawInputField(rect);
        }
    }
    private static Vector2 CalcOperatorButtonSize(FilterExpression state)
    {
        var size = Text.CalcSize(state.OperatorString);
        size.x += OperatorButtonPadding * 2f;

        return size;
    }
    private static bool DrawOperatorButton(Rect rect, FilterExpression state)
    {
        return Widgets.Draw.ButtonTextSubtle(rect, state.OperatorString, OperatorButtonPadding);
    }
    private static Vector2 CalcInputFieldSize(FilterExpression state)
    {
        var size = Text.CalcSize(state.RhsString);
        size.x += Globals.GUI.EstimatedInputFieldInnerPadding * 2f;

        return size;
    }
    protected abstract void DrawInputField(Rect rect);
    private void HandleStateChange(FilterExpression state)
    {
        OperatorButtonSize = CalcOperatorButtonSize(state);
        InputFieldSize = CalcInputFieldSize(state);

        Resize();
    }
    protected static FloatMenuOption MakeClearStateOperatorsMenuOption(FilterExpression state)
    {
        return new FloatMenuOption("Clear", state.Clear);
    }
}
