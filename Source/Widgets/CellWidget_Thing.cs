using UnityEngine;
using Verse;

namespace Stats;

internal sealed class CellWidget_Thing : ICellWidget<ThingAlike>
{
    public ThingAlike Value { get; }
    public float MinWidth { get; } = TableWidget.CellMinWidth;
    public CellWidget_Thing(ThingAlike thing)
    {
        Value = thing;
        MinWidth += TableWidget.RowHeight + TableWidget.CellPadding + Text.CalcSize(thing.Label).x;
    }
    public void Draw(Rect targetRect)
    {
        var contentRect = targetRect.ContractedBy(TableWidget.CellPadding, 0f);
        var curX = contentRect.x;

        Value.Icon.Draw(contentRect.CutFromX(ref curX, contentRect.height));
        curX += TableWidget.CellPadding;
        Text.Anchor = TextAnchor.LowerLeft;
        Widgets.Label(contentRect.CutFromX(ref curX), Value.Label);
        Text.Anchor = Constants.DefaultTextAnchor;
        Widgets.DrawHighlightIfMouseover(targetRect);

        if (Widgets.ButtonInvisible(targetRect))
        {
            DefInfoDialogWidget.Draw(Value.Def, Value.Stuff);
        }

        TooltipHandler.TipRegion(targetRect, Value.Def.description);
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.Label.CompareTo(((ICellWidget<ThingAlike>)other).Value.Label);
    }
}
