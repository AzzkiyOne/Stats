using System;
using System.Reflection;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class AbsCell<T>(T value) : IComparable<AbsCell<T>> where T : IComparable<T>
{
    public T value { get; } = value;
    public virtual void Draw(Rect targetRect)
    {
        var contentRect = targetRect.ContractedBy(Table<ThingAlike>.cellPaddingHor, 0);

        Widgets.Label(contentRect, ToString());
    }
    public int CompareTo(AbsCell<T> other)
    {
        return value.CompareTo(other.value);
    }
}

public class NumCell(float value, string valueStr) : AbsCell<float>(value)
{
    public override string ToString()
    {
        return valueStr;
    }

    public static readonly NumCell Empty = new(float.NaN, "");
}

public class StrCell(string value) : AbsCell<string>(value)
{
    public override string ToString()
    {
        return value;
    }

    public static readonly StrCell Empty = new("");
}

static class Cell
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
