using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Stats;

public interface ICell :
    IDrawable
{
    public IComparable value { get; }
}

public abstract class Cell(IComparable value) :
    ICell
{
    public IComparable value { get; } = value;
    public virtual void Draw(Rect targetRect)
    {
        var contentRect = targetRect.ContractedBy(Table<ThingAlike>.cellPaddingHor, 0);

        Widgets.Label(contentRect, ToString());
    }
}

public class NumCell(float value, string valueStr) : Cell(value)
{
    public override string ToString()
    {
        return valueStr;
    }

    public static readonly NumCell Empty = new(float.NaN, "");
}

// Do we still need this?
public class StrCell(string _value) : Cell(_value)
{
    public override string ToString()
    {
        return _value;
    }

    public static readonly StrCell Empty = new("");
}

static class CellWidgets
{
    private static readonly FieldInfo dialogInfoCardStuffField = typeof(Dialog_InfoCard)
        .GetField("stuff", BindingFlags.Instance | BindingFlags.NonPublic);

    static public void Label(Rect targetRect, string text)
    {
        var contentRect = targetRect.ContractedBy(Table<ThingAlike>.cellPaddingHor, 0);

        Widgets.Label(contentRect, text);
    }
    static public void LabelWithDefIcon(Rect targetRect, ThingAlike thing, string text)
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
        Widgets.DefIcon(iconRect, thing.def, thing.stuff);
        Widgets.Label(textRect, text);
    }
    static public void Tip(Rect targetRect, string text)
    {
        if (Event.current.control)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(text));
        }
    }
    static public void DefDialogOnClick(Rect targetRect, ThingAlike thing)
    {
        Widgets.DrawHighlightIfMouseover(targetRect);

        if (Widgets.ButtonInvisible(targetRect))
        {
            var dialog = new Dialog_InfoCard(thing.def);

            if (thing.stuff != null)
            {
                dialogInfoCardStuffField.SetValue(dialog, thing.stuff);
            }

            Find.WindowStack.Add(dialog);
        }
    }
}
