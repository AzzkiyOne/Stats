using UnityEngine;
using Verse;

namespace Stats;

internal interface IFilterWidget<T>
{
    IFilterProvider<T> Column { get; }
    bool Match(ThingAlike thing);
    bool Draw(Rect targetRect);
}

internal interface IFilterProvider<T> : IColumnDef
{
    T GetValue(ThingAlike thing);
    IFilterWidget<T> GetFilter();
}

internal class Filter_Num : IFilterWidget<float?>
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
    public IFilterProvider<float?> Column { get; }
    public Filter_Num(IFilterProvider<float?> column)
    {
        Column = column;
        Menu = new([
            new( "=", () => CurOperator = "=" ),
            new("!=", () => CurOperator = "!="),
            new( ">", () => CurOperator = ">" ),
            new( "<", () => CurOperator = "<" ),
            new(">=", () => CurOperator = ">="),
            new("<=", () => CurOperator = "<="),
        ]);
    }
    public bool Match(ThingAlike thing)
    {
        return CurOperator switch
        {
            "=" => (Column.GetValue(thing) ?? 0f) == curValue,
            "!=" => (Column.GetValue(thing) ?? 0f) != curValue,
            ">" => (Column.GetValue(thing) ?? 0f) > curValue,
            "<" => (Column.GetValue(thing) ?? 0f) < curValue,
            ">=" => (Column.GetValue(thing) ?? 0f) >= curValue,
            "<=" => (Column.GetValue(thing) ?? 0f) <= curValue,
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

internal class Filter_Bool : IFilterWidget<bool?>
{
    private bool curValue = true;
    public IFilterProvider<bool?> Column { get; }
    public Filter_Bool(IFilterProvider<bool?> column)
    {
        Column = column;
    }
    public bool Match(ThingAlike thing)
    {
        return (Column.GetValue(thing) ?? false) == curValue;
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

internal class Filter_Str : IFilterWidget<string?>
{
    private string? curValue = default;
    private string CurOperator { get; set; } = "=";
    public IFilterProvider<string?> Column { get; }
    public Filter_Str(IFilterProvider<string?> column)
    {
        Column = column;
    }
    public bool Match(ThingAlike thing)
    {
        return true;
    }
    public bool Draw(Rect targetRect)
    {
        //var curX = targetRect.x;

        return false;
    }
}
