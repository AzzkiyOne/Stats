using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Things : ICellWidget<List<ThingAlike>>
{
    public List<ThingAlike> Value { get; }
    public float MinWidth { get; }
    public CellWidget_Things(List<ThingAlike> value)
    {
        Value = value;
        MinWidth = value.Count * TableWidget.RowHeight + (Value.Count - 1) * TableWidget.CellPadding;
    }
    public void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        var curX = contentRect.x;

        for (int i = 0; i < Value.Count; i++)
        {
            var thing = Value[i];
            var iconRect = contentRect.CutFromX(ref curX, contentRect.height);

            thing.Icon.Draw(iconRect);
            Widgets.DrawHighlightIfMouseover(iconRect);
            if (Widgets.ButtonInvisible(iconRect))
            {
                DefInfoDialogWidget.Draw(thing.Def, thing.Stuff);
            }
            TooltipHandler.TipRegion(iconRect, thing.Label);

            curX += TableWidget.CellPadding;
        }
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.Count.CompareTo(((ICellWidget<List<ThingAlike>>)other).Value.Count);
    }
}
