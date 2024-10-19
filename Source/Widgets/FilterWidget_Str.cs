using System;
using UnityEngine;
using Verse;

namespace Stats;

public class FilterWidget_Str : IFilterWidget
{
    private string _curValue = "";
    private string CurValue
    {
        get => _curValue;
        set
        {
            if (_curValue != value)
            {
                _curValue = value;
                WasUpdated = true;
                CurValueLower = value.ToLower();
            }
        }
    }
    private string CurValueLower = "";
    public bool WasUpdated { get; set; } = false;
    public bool HasValue => CurOperator != Operator.Any;
    private Operator _curOperator = Operator.Any;
    private Operator CurOperator
    {
        get => _curOperator;
        set
        {
            if (_curOperator != value)
            {
                _curOperator = value;
                CurOperatorStr = value switch
                {
                    #pragma warning disable format
                    Operator.Any   => "Any",
                    Operator.Eq    => "==",
                    Operator.EqNot => "!=",
                    #pragma warning restore format
                };
                WasUpdated = true;
            }
        }
    }
    private string CurOperatorStr = "Any";
    private readonly FloatMenu OperatorsMenu;
    private readonly Func<ThingRec, string?> ValueFunc;
    public FilterWidget_Str(Func<ThingRec, string?> valueFunc)
    {
        ValueFunc = valueFunc;
        OperatorsMenu = new([
            #pragma warning disable format
            new("Any", () => CurOperator = Operator.Any),
            new( "==", () => CurOperator = Operator.Eq),
            new( "!=", () => CurOperator = Operator.EqNot),
            #pragma warning restore format
        ]);
    }
    public bool Match(ThingRec thing)
    {
        var valueLower = ValueFunc(thing)?.ToLower() ?? "";

        return CurOperator switch
        {
            #pragma warning disable format
            Operator.Any    => true,
            Operator.Eq     => CurValueLower == ""
                ? valueLower == ""
                : valueLower.Contains(CurValueLower),
            Operator.EqNot  => CurValueLower == ""
                ? valueLower != ""
                : !valueLower.Contains(CurValueLower),
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
            CurValue = Widgets.TextField(targetRect, CurValue);
        }
    }
    private enum Operator
    {
        Any,
        Eq,
        EqNot,
    }
}
