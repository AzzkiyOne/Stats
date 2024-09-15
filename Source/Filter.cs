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
    void Draw(Rect targetRect);
}

public interface IFilter<T> : IFilter
{
    bool Match(T obj);
}

public class FilterNum<T> : IFilter<T>
{
    private float curValue = default;
    private string curValueStrBuffer = "";
    private string CurOperator { get; set; } = "=";
    private FloatMenu Menu { get; }
    Func<T, float?> GetValue { get; }
    private GenTable.ColumnDef Column { get; }
    public FilterNum(GenTable.ColumnDef column, Func<T, float?> getValue)
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
        if (curValueStrBuffer == "")
        {
            return CurOperator switch
            {
                "=" => GetValue(obj) == null,
                "!=" => GetValue(obj) != null,
                _ => true,
            };
        }

        return CurOperator switch
        {
            "=" => GetValue(obj) == curValue,
            "!=" => GetValue(obj) != curValue,
            ">" => GetValue(obj) > curValue,
            "<" => GetValue(obj) < curValue,
            ">=" => GetValue(obj) >= curValue,
            "<=" => GetValue(obj) <= curValue,
            //_ => throw new NotImplementedException("Unknown operator."),
            _ => true,
        };
    }
    public void Draw(Rect targetRect)
    {
        var curX = targetRect.x;

        using (new TextAnchorCtx(TextAnchor.LowerRight))
        {
            Widgets.Label(targetRect.CutFromX(ref curX, targetRect.width / 3).ContractedBy(5f, 0f), Column.Label);
        }

        if (Widgets.ButtonText(targetRect.CutFromX(ref curX, targetRect.height), CurOperator))
        {
            Find.WindowStack.Add(Menu);
        }

        Widgets.TextFieldNumeric(targetRect.CutFromX(ref curX), ref curValue, ref curValueStrBuffer);
    }
}
