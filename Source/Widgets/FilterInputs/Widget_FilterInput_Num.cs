using System;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Widget_FilterInput_Num
    : IWidget_FilterInput
{
    private float CurValue = 0f;
    private string CurValueStrBuffer = "";
    public bool WasUpdated { get; set; } = false;
    public bool HasValue => CurOperator != Operator.Any;
    private Operator _curOperator = Operator.Any;
    private Operator CurOperator
    {
        get => _curOperator;
        set
        {
            if (_curOperator == value) return;

            _curOperator = value;
            CurOperatorStr = value switch
            {
                #pragma warning disable format
                Operator.Any    => "Any",
                Operator.Eq     => "==",
                Operator.EqNot  => "!=",
                Operator.GT     => ">",
                Operator.LT     => "<",
                Operator.GTorEq => ">=",
                Operator.LTorEq => "<=",
                _               => throw new NotImplementedException(),
                #pragma warning restore format
            };
            WasUpdated = true;
        }
    }
    private string CurOperatorStr = "Any";
    private readonly FloatMenu OperatorsMenu;
    private readonly Func<ThingRec, float> ValueFunc;
    public Widget_FilterInput_Num(Func<ThingRec, float> valueFunc)
    {
        ValueFunc = valueFunc;
        OperatorsMenu = new([
            #pragma warning disable format
            new("Any", () => CurOperator = Operator.Any),
            new(  "=", () => CurOperator = Operator.Eq),
            new( "!=", () => CurOperator = Operator.EqNot),
            new(  ">", () => CurOperator = Operator.GT),
            new(  "<", () => CurOperator = Operator.LT),
            new( ">=", () => CurOperator = Operator.GTorEq),
            new( "<=", () => CurOperator = Operator.LTorEq),
            #pragma warning restore format
        ]);
    }
    public bool Match(ThingRec thing)
    {
        var value = ValueFunc(thing);

        return CurOperator switch
        {
            #pragma warning disable format
            Operator.Any    => true,
            Operator.Eq     => value == CurValue,
            Operator.EqNot  => value != CurValue,
            Operator.GT     => value >  CurValue,
            Operator.LT     => value <  CurValue,
            Operator.GTorEq => value >= CurValue,
            Operator.LTorEq => value <= CurValue,
            _               => throw new NotImplementedException(),
            #pragma warning restore format
        };
    }
    public void Draw(Rect targetRect)
    {
        if (Widgets.ButtonTextSubtle(
            CurOperator == Operator.Any
                ? targetRect
                : targetRect.CutByX(targetRect.height),
            CurOperatorStr
        ))
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        if (CurOperator != Operator.Any)
        {
            var num = CurValue;

            Widgets.TextFieldNumeric(targetRect, ref num, ref CurValueStrBuffer);

            if (CurValue != num)
            {
                CurValue = num;
                WasUpdated = true;
            }
        }
    }
    private enum Operator
    {
        Any,
        Eq,
        EqNot,
        GT,
        LT,
        GTorEq,
        LTorEq,
    }
}
