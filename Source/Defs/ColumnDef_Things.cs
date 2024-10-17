using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class ColumnDef_Things : ColumnDef
{
    public PropDelegate<List<ThingAlike>> prop;
    private readonly Dictionary<
        (ThingDef thingDef, ThingDef? stuffDef),
        CellWidget_Things?
    > Cells = [];
    internal override ICellWidget? GetCellWidget(ThingDef thingDef, ThingDef? stuffDef)
    {
        var key = (thingDef, stuffDef);
        var exists = Cells.TryGetValue(key, out var cell);

        if (exists == false)
        {
            var value = prop(thingDef, stuffDef);

            if (value.Count > 0)
            {
                cell = new(value);
            }

            Cells[key] = cell;
        }

        return cell;
    }
}
