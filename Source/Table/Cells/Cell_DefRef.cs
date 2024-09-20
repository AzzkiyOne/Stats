using UnityEngine;
using Verse;

namespace Stats.Table.Cells;

public sealed class Cell_DefRef : ICell<string>
{
    public string Value { get; }
    private Def Def { get; }
    private ThingDef? Stuff { get; }
    public Cell_DefRef(string value, Def def, ThingDef? stuff = null)
    {
        Value = value;
        Def = def;
        Stuff = stuff;
    }
    public void Draw(Rect targetRect, Rect contentRect, TextAnchor textAnchor)
    {
        var currX = contentRect.x;

        // This is very expensive.
        Widgets.DefIcon(contentRect.CutFromX(ref currX, contentRect.height), Def, Stuff);

        currX += GenUI.Pad;

        if (Widgets.ButtonInvisible(targetRect))
        {
            GUIWidgets.DefInfoDialog(Def, Stuff);
        }

        using (new TextAnchorCtx(textAnchor))
        {
            Widgets.Label(contentRect.CutFromX(ref currX), Value);
        }

        Widgets.DrawHighlightIfMouseover(targetRect);
        TooltipHandler.TipRegion(targetRect, new TipSignal(Def.description));
    }
    public int CompareTo(ICell? other)
    {
        if (other == null)
        {
            return 1;
        }

        return Value.CompareTo(((ICell<string>)other).Value);
    }
}
