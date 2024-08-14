using UnityEngine;
using Verse;

namespace Stats;

static class Cell
{
    static public void Label(Rect targetRect, string text)
    {
        var contentRect = targetRect.ContractedBy(Table.cellPaddingHor, 0);

        Widgets.Label(contentRect, text);
    }
    static public void LabelWithDefIcon(Rect targetRect, FakeThing thing, string text)
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
        Widgets.DefIcon(iconRect, thing.thingDef, thing.stuffDef);
        Widgets.Label(textRect, text);
    }
    static public void Tip(Rect targetRect, string text)
    {
        if (Event.current.control)
        {
            TooltipHandler.TipRegion(targetRect, new TipSignal(text));
        }
    }
    static public void DefDialogOnClick(Rect targetRect, ThingDef thingDef)
    {
        Widgets.DrawHighlightIfMouseover(targetRect);

        if (Widgets.ButtonInvisible(targetRect))
        {
            Find.WindowStack.Add(new Dialog_InfoCard(thingDef));
        }
    }
}
