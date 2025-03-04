using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Widget_Things_Temp : IWidget
{
    private readonly IEnumerable<Widget_ThingIcon> Icons;
    public float MinWidth =>
        Icons.Count() * Widget_Table.RowHeight
        + (Icons.Count() - 1) * Widget_Table.IconGap;
    public Widget_Things_Temp(IEnumerable<ThingDef> defs)
    {
        Icons = defs
            .OrderBy(def => def.label)
            .Select(def => new Widget_ThingIcon(def));
    }
    public void Draw(Rect targetRect)
    {
        foreach (var icon in Icons)
        {
            icon.Draw(targetRect.CutByX(targetRect.height));
            targetRect.PadLeft(Widget_Table.IconGap);
        }
    }
}
