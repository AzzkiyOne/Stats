using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class CellWidget_Things : ICellWidget
{
    private readonly IEnumerable<ThingIconWidget> Icons;
    public float MinWidth => (Icons.Count() * TableWidget_Base.RowHeight)
                             + ((Icons.Count() - 1) * TableWidget_Base.IconGap);
    public CellWidget_Things(IEnumerable<ThingRec> things)
    {
        Icons = things
            .OrderBy(thing => thing.Def.label)
            .Select(thing => new ThingIconWidget(thing));
    }
    public CellWidget_Things(IEnumerable<ThingDef> defs)
    {
        Icons = defs
            .OrderBy(def => def.label)
            .Select(def => new ThingIconWidget(def));
    }
    public void Draw(Rect targetRect)
    {
        var contentRect = targetRect.ContractedBy(TableWidget_Base.CellPadding, 0f);

        foreach (var icon in Icons)
        {
            icon.Draw(contentRect.CutByX(contentRect.height));
            contentRect.PadLeft(TableWidget_Base.IconGap);
        }
    }
}
