using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

public class Widget_FilterInput_Str : IWidget_FilterInput
{
    private string _curValue = "";
    private string CurValue
    {
        get => _curValue;
        set
        {
            if (_curValue == value) return;

            _curValue = value;
            WasUpdated = true;
            CurValueLower = value.ToLower();
            CurValueLowerStrs.Clear();
            CurValueLowerStrs.AddRange(CurValueLower.Split(','));
        }
    }
    private string CurValueLower = "";
    private readonly List<string> CurValueLowerStrs = [];
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
                Operator.Any   => "Any",
                Operator.Eq    => "==",
                Operator.EqNot => "!=",
                _              => throw new NotImplementedException(),
                #pragma warning restore format
            };
            WasUpdated = true;
        }
    }
    private string CurOperatorStr = "Any";
    private readonly FloatMenu OperatorsMenu;
    private readonly Func<ThingRec, string?> ValueFunc;
    private const string Description = "Use \",\" to search by multiple terms.";
    public Widget_FilterInput_Str(Func<ThingRec, string?> valueFunc)
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
                : CurValueLowerStrs.Count > 0
                ? CurValueLowerStrs.Any(valueLower.Contains)
                : valueLower.Contains(CurValueLower),
            Operator.EqNot  => CurValueLower == ""
                ? valueLower != ""
                : CurValueLowerStrs.Count > 0
                ? !CurValueLowerStrs.Any(valueLower.Contains)
                : !valueLower.Contains(CurValueLower),
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
            CurValue = Widgets.TextField(targetRect, CurValue);
            TooltipHandler.TipRegion(targetRect, Description);
        }
    }
    private enum Operator
    {
        Any,
        Eq,
        EqNot,
    }
}
