using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Widget_FilterInput_Num
    : Widget,
      IWidget_FilterInput
{
    protected override Vector2 Size { get; set; }
    private readonly ThingMatcher_Num _ThingMatcher;
    public IThingMatcher ThingMatcher => _ThingMatcher;
    private readonly FloatMenu OperatorsMenu;
    public Widget_FilterInput_Num(ThingMatcher_Num thingMatcher)
    {
        _ThingMatcher = thingMatcher;
        var menuOptions = new List<FloatMenuOption>(Operatos.Count);
        foreach (var op in Operatos)
        {
            var optText = op.ToString();
            void optCb() => _ThingMatcher.Operator = op;
            menuOptions.Add(new FloatMenuOption(optText, optCb));
        }
        OperatorsMenu = new FloatMenu(menuOptions);
        Size = GetSize();
        thingMatcher.OnChange += UpdateSize;
    }
    public override Vector2 GetSize()
    {
        if (_ThingMatcher.Operator is Op_Float_Any)
        {
            return Text.CalcSize(_ThingMatcher.Operator.ToString());
        }

        var size = Text.CalcSize(
            _ThingMatcher.Operator.ToString() + _ThingMatcher.ValueStrBuffer
        );
        size.x += Constants.EstimatedInputFieldInnerPadding * 2;

        return size;
    }
    protected override void DrawContent(Rect rect)
    {
        if
        (
            Widgets.ButtonTextSubtle(
                _ThingMatcher.Operator is Op_Float_Any
                    ? rect
                    : rect.CutByX(rect.height),
                _ThingMatcher.Operator.ToString()
            )
        )
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (_ThingMatcher.Operator is not Op_Float_Any)
        {
            var num = _ThingMatcher.Value;
            Widgets.TextFieldNumeric(rect, ref num, ref _ThingMatcher.ValueStrBuffer);
            _ThingMatcher.Value = num;
        }
    }
    public IWidget_FilterInput Clone()
    {
        return new Widget_FilterInput_Num(_ThingMatcher);
    }

    public sealed class ThingMatcher_Num
        : ThingMatcher<float>
    {
        public string ValueStrBuffer = "";
        public ThingMatcher_Num(Func<ThingRec, float> valueFunc)
            : base(0f, Op_Float_Any.Instance, valueFunc)
        {
        }
        public override void Reset()
        {
            base.Reset();

            ValueStrBuffer = "";
        }
    }

    private sealed class Op_Float_Any
        : IBinaryOp<float>
    {
        private Op_Float_Any() { }
        public bool Eval(float lhs, float rhs) => true;
        public override string ToString() => "Any";
        public static IBinaryOp<float> Instance { get; } = new Op_Float_Any();
    }

    private sealed class Op_Float_Eq
        : IBinaryOp<float>
    {
        private Op_Float_Eq() { }
        public bool Eval(float lhs, float rhs) => lhs == rhs;
        public override string ToString() => "==";
        public static IBinaryOp<float> Instance { get; } = new Op_Float_Eq();
    }

    private sealed class Op_Float_EqNot
        : IBinaryOp<float>
    {
        private Op_Float_EqNot() { }
        public bool Eval(float lhs, float rhs) => lhs != rhs;
        public override string ToString() => "!=";
        public static IBinaryOp<float> Instance { get; } = new Op_Float_EqNot();
    }

    private sealed class Op_Float_Gt
        : IBinaryOp<float>
    {
        private Op_Float_Gt() { }
        public bool Eval(float lhs, float rhs) => lhs > rhs;
        public override string ToString() => ">";
        public static IBinaryOp<float> Instance { get; } = new Op_Float_Gt();
    }

    private sealed class Op_Float_Lt
        : IBinaryOp<float>
    {
        private Op_Float_Lt() { }
        public bool Eval(float lhs, float rhs) => lhs < rhs;
        public override string ToString() => "<";
        public static IBinaryOp<float> Instance { get; } = new Op_Float_Lt();
    }

    private sealed class Op_Float_GtOrEq
        : IBinaryOp<float>
    {
        private Op_Float_GtOrEq() { }
        public bool Eval(float lhs, float rhs) => lhs >= rhs;
        public override string ToString() => ">=";
        public static IBinaryOp<float> Instance { get; } = new Op_Float_GtOrEq();
    }

    private sealed class Op_Float_LtOrEq
        : IBinaryOp<float>
    {
        private Op_Float_LtOrEq() { }
        public bool Eval(float lhs, float rhs) => lhs <= rhs;
        public override string ToString() => "<=";
        public static IBinaryOp<float> Instance { get; } = new Op_Float_LtOrEq();
    }

    private static readonly ReadOnlyCollection<IBinaryOp<float>> Operatos =
        new([
            Op_Float_Any.Instance,
            Op_Float_Eq.Instance,
            Op_Float_EqNot.Instance,
            Op_Float_Gt.Instance,
            Op_Float_Lt.Instance,
            Op_Float_GtOrEq.Instance,
            Op_Float_LtOrEq.Instance,
        ]);
}
