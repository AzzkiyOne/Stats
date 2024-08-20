using System.Reflection;
using UnityEngine;
using Verse;

namespace Stats;

static class Cell
{
    private static readonly FieldInfo dialogInfoCardStuffField = typeof(Dialog_InfoCard)
        .GetField("stuff", BindingFlags.Instance | BindingFlags.NonPublic);

    static public void Label(Rect targetRect, string text)
    {
        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);

        Widgets.Label(contentRect, text);
    }
    static public void LabelWithDefIcon(Rect targetRect, ThingAlike thing, string text)
    {
        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);
        var iconRect = new Rect(
            contentRect.x,
            contentRect.y,
            contentRect.height,
            contentRect.height
        );
        var textRect = new Rect(
            iconRect.xMax + Table.cellPaddingHor,
            contentRect.y,
            contentRect.width - iconRect.width - Table.cellPaddingHor,
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
