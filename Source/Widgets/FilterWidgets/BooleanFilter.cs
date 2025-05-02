using System;
using Stats.RelationalOperators;
using UnityEngine;
using Verse;

namespace Stats.Widgets.FilterWidgets;

public sealed class BooleanFilter : FilterWidget
{
    private readonly BooleanFilterState SharedState;
    public override FilterExpression State => State;
    private Action<Rect> DrawValue;
    private BooleanFilter(BooleanFilterState state)
    {
        SharedState = state;
        DrawValue = DrawEmpty;

        state.OnChange += HandleStateChange;
    }
    public BooleanFilter(Func<ThingAlike, bool> lhs) : this(new BooleanFilterState(lhs))
    {
    }
    private void HandleStateChange(FilterExpression _)
    {
        DrawValue = SharedState switch
        {
            { IsEmpty: true } => DrawEmpty,
            { Rhs: true } => DrawTrue,
            { Rhs: false } => DrawFalse,
        };

        Resize();
    }
    protected override Vector2 CalcSize()
    {
        if (State.IsEmpty)
        {
            return Text.CalcSize(SharedState.Operator.ToString());
        }

        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        DrawValue(rect);
    }
    private void DrawEmpty(Rect rect)
    {
        if (Widgets.Draw.ButtonTextSubtle(rect, SharedState.Operator.ToString()))
        {
            SharedState.Operator = BooleanFilterState.Operators.EqualTo;
        }
    }
    private void DrawTrue(Rect rect)
    {
        if (Widgets.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOnTex))
        {
            SharedState.Rhs = false;
        }
    }
    private void DrawFalse(Rect rect)
    {
        if (Widgets.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOffTex))
        {
            State.Clear();
        }
    }
    public override FilterWidget Clone()
    {
        return new BooleanFilter(SharedState);
    }

    private sealed class BooleanFilterState : FilterExpression<bool, bool>
    {
        public BooleanFilterState(Func<ThingAlike, bool> lhs) : base(lhs, true)
        {
        }
        public override void Clear()
        {
            base.Clear();

            Rhs = true;
        }

        public static class Operators
        {
            public sealed class EqualToOperator : RelationalOperator<bool, bool>
            {
                private EqualToOperator() { }
                public override bool Eval(bool lhs, bool rhs) => lhs == rhs;
                public override string ToString() => "==";
                public static EqualToOperator Instance { get; } = new();
            }
            public static EqualToOperator EqualTo = EqualToOperator.Instance;
        }
    }
}
