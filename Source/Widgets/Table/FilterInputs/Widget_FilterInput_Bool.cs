using System;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Widget_FilterInput_Bool
    : Widget,
      IWidget_FilterInput
{

    protected override Vector2 Size { get; set; }
    private readonly ThingMatcher_Bool _ThingMatcher;
    public IThingMatcher ThingMatcher => _ThingMatcher;
    public Widget_FilterInput_Bool(ThingMatcher_Bool thingMatcher)
    {
        _ThingMatcher = thingMatcher;
        Size = GetSize();
    }
    public override Vector2 GetSize()
    {
        if (_ThingMatcher.Operator is Op_Bool_Any)
        {
            return Text.CalcSize(_ThingMatcher.Operator.ToString());
        }

        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
    protected override void DrawContent(Rect rect)
    {
        switch (_ThingMatcher)
        {
            case { Operator: Op_Bool_Any }:
                if (Widgets.ButtonTextSubtle(rect, "Any"))
                {
                    _ThingMatcher.Set(true, Op_Bool_Eq.Instance);
                }
                break;
            case { Value: true }:
                if (Widgets.ButtonImageFitted(rect, Widgets.CheckboxOnTex))
                {
                    _ThingMatcher.Value = false;
                }
                break;
            case { Value: false }:
                if (Widgets.ButtonImageFitted(rect, Widgets.CheckboxOffTex))
                {
                    _ThingMatcher.Operator = Op_Bool_Any.Instance;
                }
                break;
        }
    }
    public IWidget_FilterInput Clone()
    {
        return new Widget_FilterInput_Bool(_ThingMatcher);
    }

    public sealed class ThingMatcher_Bool
        : ThingMatcher<bool>
    {
        public ThingMatcher_Bool(Func<ThingRec, bool> valueFunc)
            : base(true, Op_Bool_Any.Instance, valueFunc)
        {
        }
    }

    private sealed class Op_Bool_Any
        : IBinaryOp<bool>
    {
        private Op_Bool_Any() { }
        public bool Eval(bool lhs, bool rhs) => true;
        public override string ToString() => "Any";
        public static IBinaryOp<bool> Instance { get; } = new Op_Bool_Any();
    }

    private sealed class Op_Bool_Eq
        : IBinaryOp<bool>
    {
        private Op_Bool_Eq() { }
        public bool Eval(bool lhs, bool rhs) => lhs == rhs;
        public override string ToString() => "==";
        public static IBinaryOp<bool> Instance { get; } = new Op_Bool_Eq();
    }
}
