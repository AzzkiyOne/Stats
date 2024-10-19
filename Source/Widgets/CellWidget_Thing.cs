using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Thing : ICellWidget<ThingAlike>
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
        Widgets.DrawHighlightIfMouseover(targetRect);
        TooltipHandler.TipRegion(targetRect, Value.Def.description);

        if (Widgets.ButtonInvisible(targetRect))
        {
            DefInfoDialogWidget.Draw(Value.Def, Value.Stuff);
        }

        var contentRect = targetRect.ContractedBy(TableWidget.CellPadding, 0f);

        Value.Icon.Draw(contentRect.CutByX(contentRect.height));
        contentRect.PadLeft(TableWidget.CellPadding);
        Text.Anchor = TextAnchor.LowerLeft;
        Widgets.Label(contentRect, Value.Label);
        Text.Anchor = Constants.DefaultTextAnchor;
    }
    public int CompareTo(ICellWidget? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.Label.CompareTo(((ICellWidget<ThingAlike>)other).Value.Label);
    }
    public override string ToString()
    {
        return Value.Label;
    }
}
