using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Things : ICellWidget<List<ThingAlike>>
{
    public List<ThingAlike> Value { get; }
    private readonly string ValueStr;
    public float MinWidth { get; } = TableWidget_Base.CellMinWidth;
    public CellWidget_Things(List<ThingAlike> value)
    {
        Value = value;
        MinWidth += value.Count * TableWidget_Base.RowHeight + (Value.Count - 1) * TableWidget_Base.IconGap;
        Value.SortBy(thing => thing.Label);
        ValueStr = string.Join(", ", Value.Select(thing => thing.Label));
    }
    public void Draw(Rect targetRect)
    {
        var contentRect = targetRect.ContractedBy(TableWidget_Base.CellPadding, 0f);

        for (int i = 0; i < Value.Count; i++)
        {
            var thing = Value[i];
            var iconRect = contentRect.CutByX(contentRect.height);

            thing.Icon.Draw(iconRect);
            Widgets.DrawHighlightIfMouseover(iconRect);

            if (Widgets.ButtonInvisible(iconRect))
            {
                DefInfoDialogWidget.Draw(thing.Def, thing.Stuff);
            }

            TooltipHandler.TipRegion(iconRect, thing.Label);
            contentRect.PadLeft(TableWidget_Base.IconGap);
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
    public override string ToString()
    {
        return ValueStr;
    }
}
