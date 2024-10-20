using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Thing : ICellWidget<ThingAlike>
{
    public ThingAlike Value { get; }
    public float MinWidth { get; } = TableWidget_Base.CellMinWidth;
    public CellWidget_Thing(ThingAlike thing)
    {
        Value = thing;
        MinWidth += TableWidget_Base.RowHeight + TableWidget_Base.IconGap + Text.CalcSize(thing.Label).x;
    }
    public void Draw(Rect targetRect)
    {
        Widgets.DrawHighlightIfMouseover(targetRect);
        TooltipHandler.TipRegion(targetRect, Value.Def.description);

        if (Widgets.ButtonInvisible(targetRect))
        {
            DefInfoDialogWidget.Draw(Value.Def, Value.Stuff);
        }

        var contentRect = targetRect.ContractedBy(TableWidget_Base.CellPadding, 0f);

        Value.Icon.Draw(contentRect.CutByX(contentRect.height));
        contentRect.PadLeft(TableWidget_Base.IconGap);
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
