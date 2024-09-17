using System;
using UnityEngine;
using Verse;

namespace Stats;

public interface IFilterProvider<T>
{
    IFilter<T> GetFilter();
}

public interface IFilter
{
    GenTable.IColumn Column { get; }
    bool Draw(Rect targetRect);
}

public interface IFilter<T> : IFilter
{
    bool Match(T obj);
}

public class Filter_Num<T> : IFilter<T>
{
    private float curValue = 0f;
    private string curValueStrBuffer = "";
    private bool WasUpdated { get; set; } = false;
    private string curOperator = "=";
    private string CurOperator
    {
        get => curOperator;
        set
        {
            if (curOperator != value)
            {
                curOperator = value;
                WasUpdated = true;
            }
        }
    }
    private FloatMenu Menu { get; }
    private Func<T, float?> GetValue { get; }
    public GenTable.IColumn Column { get; }
    public Filter_Num(GenTable.IColumn column, Func<T, float?> getValue)
    {
        Column = column;
        GetValue = getValue;
        Menu = new([
            new( "=", () => CurOperator = "=" ),
            new("!=", () => CurOperator = "!="),
            new( ">", () => CurOperator = ">" ),
            new( "<", () => CurOperator = "<" ),
            new(">=", () => CurOperator = ">="),
            new("<=", () => CurOperator = "<="),
        ]);
    }
    public bool Match(T obj)
    {
        return CurOperator switch
        {
            "=" => (GetValue(obj) ?? 0f) == curValue,
            "!=" => (GetValue(obj) ?? 0f) != curValue,
            ">" => (GetValue(obj) ?? 0f) > curValue,
            "<" => (GetValue(obj) ?? 0f) < curValue,
            ">=" => (GetValue(obj) ?? 0f) >= curValue,
            "<=" => (GetValue(obj) ?? 0f) <= curValue,
            //_ => throw new NotImplementedException("Unknown operator."),
            _ => true,
        };
    }
    public bool Draw(Rect targetRect)
    {
        var curX = targetRect.x;

        if (Widgets.ButtonText(targetRect.CutFromX(ref curX, targetRect.height), CurOperator))
        {
            Find.WindowStack.Add(Menu);
        }
        curX += 5f;

        var curValue = this.curValue;

        Widgets.TextFieldNumeric(targetRect.CutFromX(ref curX), ref curValue, ref curValueStrBuffer);

        if (this.curValue != curValue)
        {
            this.curValue = curValue;
            WasUpdated = true;
        }

        if (WasUpdated)
        {
            WasUpdated = false;

            return true;
        }

        return false;
    }
}

public class Filter_Bool<T> : IFilter<T>
{
    private bool curValue = true;
    private Func<T, bool?> GetValue { get; }
    public GenTable.IColumn Column { get; }
    public Filter_Bool(GenTable.IColumn column, Func<T, bool?> getValue)
    {
        Column = column;
        GetValue = getValue;
    }
    public bool Match(T obj)
    {
        return (GetValue(obj) ?? false) == curValue;
    }
    public bool Draw(Rect targetRect)
    {
        var curX = targetRect.x;
        var trueRect = targetRect.CutFromX(ref curX, targetRect.width / 2);
        var falseRect = targetRect.CutFromX(ref curX);

        Widgets.DrawHighlight(curValue ? trueRect : falseRect);

        Widgets.DrawTextureFitted(trueRect, Widgets.CheckboxOnTex, 0.7f);
        Widgets.DrawTextureFitted(falseRect, Widgets.CheckboxOffTex, 0.7f);

        Widgets.DrawHighlightIfMouseover(!curValue ? trueRect : falseRect);
        if (Widgets.ButtonInvisible(!curValue ? trueRect : falseRect))
        {
            curValue = !curValue;

            return true;
        }

        return false;
    }
}

public class Filter_Str<T> : IFilter<T>
{
    private string? curValue = default;
    private string CurOperator { get; set; } = "=";
    private Func<T, string?> GetValue { get; }
    public GenTable.IColumn Column { get; }
    public Filter_Str(GenTable.IColumn column, Func<T, string?> getValue)
    {
        Column = column;
        GetValue = getValue;
    }
    public bool Match(T obj)
    {
        return true;
    }
    public bool Draw(Rect targetRect)
    {
        var curX = targetRect.x;

        return false;
    }
}