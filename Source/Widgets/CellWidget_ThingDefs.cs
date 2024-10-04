using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_ThingDefs : CellWidget_Base<List<ThingDef>>
{
    public CellWidget_ThingDefs(List<ThingDef> value) : base(value, "")
    {
        MinWidth = Value.Count * TableWidget.RowHeight + (Value.Count - 1) * TableWidget.CellPadding;
    }
    public override void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        var currX = contentRect.x;

        foreach (var def in Value)
        {
            var iconRect = contentRect.CutFromX(ref currX, contentRect.height);

            Widgets.DefIcon(iconRect, def);
            Widgets.DrawHighlightIfMouseover(iconRect);
            if (Widgets.ButtonInvisible(iconRect))
            {
                DefInfoDialogWidget.Draw(def);
            }
            TooltipHandler.TipRegion(iconRect, def.LabelCap);

            currX += TableWidget.CellPadding;
        }
    }
    public override int CompareTo(ICellWidget<List<ThingDef>>? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.Count.CompareTo(other.Value.Count);
    }
}
