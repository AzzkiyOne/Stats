using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class ColumnDef_Thing : ColumnDef
{
    public PropDelegate<ThingAlike?> prop = (ThingDef thingDef, ThingDef? stuffDef) => new ThingAlike(thingDef, stuffDef);
    private readonly Dictionary<
        (ThingDef thingDef, ThingDef? stuffDef),
        CellWidget_Thing?
    > Cells = [];
    internal override ICellWidget? GetCellWidget(ThingDef thingDef, ThingDef? stuffDef)
    {
        var key = (thingDef, stuffDef);
        var exists = Cells.TryGetValue(key, out var cell);

        if (exists == false)
        {
            var value = prop(thingDef, stuffDef);

            if (value != null)
            {
                cell = new(value);
            }

            Cells[key] = cell;
        }

        return cell;
    }
}
