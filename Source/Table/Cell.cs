using System;
using UnityEngine;
using Verse;

namespace Stats;

public interface ICell : IComparable<ICell>
{
    public IComparable? value { get; }
    public void Draw(Rect targetRect);
}

public class Cell : ICell
{
    public IComparable? value { get; }
    private readonly string valueStr;
    public Def? def { get; init; }
    public ThingDef? stuff { get; init; }
    public string? tip { get; init; }

    public Cell(IComparable? value = null, string? valueStr = "")
    {
        this.value = value;

        if (!string.IsNullOrEmpty(valueStr))
        {
            this.valueStr = valueStr!;
        }
        else if (!string.IsNullOrEmpty(value?.ToString()))
        {
            this.valueStr = value!.ToString();
        }
        else
        {
            this.valueStr = valueStr!;
        }
    }

    public void Draw(Rect targetRect)
    {
        var label = ToString();

        if (label == "")
        {
            return;
        }

        var contentRect = targetRect.ContractedBy(Table<ThingAlike>.cellPaddingHor, 0);
        var currX = contentRect.x;

        if (def is not null)
        {
            // This is very expensive.
            Widgets.DefIcon(contentRect.CutFromX(ref currX, contentRect.height), def, stuff);

            currX += Table<ThingAlike>.cellPaddingHor;

            Widgets.DrawHighlightIfMouseover(targetRect);

            if (Widgets.ButtonInvisible(targetRect))
            {
                GUIWidgets.DefInfoDialog(def, stuff);
            }
        }

        Widgets.Label(contentRect.CutFromX(ref currX), label);

        if (
            //Event.current.control &&
            !string.IsNullOrEmpty(tip)
        )
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(tip));
        }
    }
    public int CompareTo(ICell other)
    {
        if (value is null)
        {
            if (other.value is null)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        else
        {
            if (other.value is null)
            {
                return 1;
            }
            else
            {
                return value.CompareTo(other.value);
            }
        }
    }

    public override string ToString()
    {
        return valueStr;
    }

    public static readonly Cell Empty = new();
}
