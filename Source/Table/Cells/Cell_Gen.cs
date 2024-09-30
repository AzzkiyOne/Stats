using System;
using UnityEngine;
using Verse;

namespace Stats.Table.Cells;

internal sealed class Cell_Gen<T> : Cell<T> where T : notnull, IComparable<T>
{
    private string Tip { get; }
    private ThingIcon? Icon { get; }
    private ThingAlike? Thing { get; }
    private Color Color { get; }
    public Cell_Gen(
        T value,
        string text,
        string tip = "",
        ThingIcon? icon = null,
        ThingAlike? thing = null,
        Color? color = null
    ) : base(value, text)
    {
        Tip = tip;
        Icon = icon;
        Thing = thing;
        Color = color ?? Color.white;
    }
    public override void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        var currX = contentRect.x;

        if (Icon != null)
        {
            Icon.Draw(contentRect.CutFromX(ref currX, contentRect.height));
            currX += GenUI.Pad;
        }

        using (new ColorCtx(Color))
        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(contentRect.CutFromX(ref currX), Text);
        }

        if (Thing != null)
        {
            Widgets.DrawHighlightIfMouseover(targetRect);

            if (Widgets.ButtonInvisible(targetRect))
            {
                GUIWidgets.DefInfoDialog(Thing.Def, Thing.Stuff);
            }
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(Tip));
    }
}
