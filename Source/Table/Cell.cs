using System;
using UnityEngine;
using Verse;

namespace Stats;

public interface ICell : Utils.IDrawable, IComparable<ICell>
{
    public IComparable? value { get; }
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
        if (def is not null)
        {
            CellWidgets.LabelWithDefIcon(targetRect, ToString(), def, stuff);
            CellWidgets.DefDialogOnClick(targetRect, def, stuff);
        }
        else
        {
            CellWidgets.Label(targetRect, ToString());
        }

        if (!string.IsNullOrEmpty(tip))
        {
            CellWidgets.Tip(targetRect, tip!);
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

static class CellWidgets
{
    static public void Label(Rect targetRect, string text)
    {
        var contentRect = targetRect.ContractedBy(Table<ThingAlike>.cellPaddingHor, 0);

        Widgets.Label(contentRect, text);
    }
    static public void LabelWithDefIcon(Rect targetRect, string text, Def def, ThingDef? stuff = null)
    {
        var contentRect = targetRect.ContractedBy(Table<ThingAlike>.cellPaddingHor, 0);
        var iconRect = new Rect(
            contentRect.x,
            contentRect.y,
            contentRect.height,
            contentRect.height
        );
        var textRect = new Rect(
            iconRect.xMax + Table<ThingAlike>.cellPaddingHor,
            contentRect.y,
            contentRect.width - iconRect.width - Table<ThingAlike>.cellPaddingHor,
            contentRect.height
        );

        //Widgets.DrawTextureFitted(iconRect, icon, 0.9f);
        // This is very expensive.
        Widgets.DefIcon(iconRect, def, stuff);
        Widgets.Label(textRect, text);
    }
    static public void Tip(Rect targetRect, string text)
    {
        if (Event.current.control)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(text));
        }
    }
    static public void DefDialogOnClick(Rect targetRect, Def def, ThingDef? stuff = null)
    {
        Widgets.DrawHighlightIfMouseover(targetRect);

        if (Widgets.ButtonInvisible(targetRect))
        {
            var dialog = new Dialog_InfoCard(def);

            if (stuff is not null)
            {
                Utils.Reflection.dialogInfoCardStuffField.SetValue(dialog, stuff);
            }

            Find.WindowStack.Add(dialog);
        }
    }
}
