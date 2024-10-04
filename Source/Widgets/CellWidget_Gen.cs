using System;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Gen<T> : CellWidget_Base<T> where T : notnull, IComparable<T>
{
    private string Tip { get; }
    private ThingIconWidget? Icon { get; }
    private ThingAlike? Thing { get; }
    private Color Color { get; }
    public CellWidget_Gen(
        T value,
        string text,
        string tip = "",
        ThingIconWidget? icon = null,
        ThingAlike? thing = null,
        Color? color = null
    ) : base(value, text)
    {
        Tip = tip;
        Icon = icon;
        Thing = thing;
        Color = color ?? Color.white;

        if (Icon != null)
        {
            MinWidth += TableWidget.RowHeight + TableWidget.CellPadding;
        }
    }
    public override void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        var currX = contentRect.x;

        if (Icon != null)
        {
            Icon.Draw(contentRect.CutFromX(ref currX, contentRect.height));
            currX += TableWidget.CellPadding;
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
                DefInfoDialogWidget.Draw(Thing.Def, Thing.Stuff);
            }
        }

        TooltipHandler.TipRegion(targetRect, Tip);
    }
    public override int CompareTo(ICellWidget<T>? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(other.Value);
    }
}
