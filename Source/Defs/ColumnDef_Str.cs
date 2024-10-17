using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats;

public sealed class ColumnDef_Str : ColumnDef
{
    public PropDelegate<string?>? prop;
    private readonly Dictionary<
        (ThingDef thingDef, ThingDef? stuffDef),
        CellWidget_Str?
    > Cells = [];
    private string? GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        if (prop != null)
        {
            return prop(thingDef, stuffDef);
        }
        else if (stat != null)
        {
            var statReq = StatRequest.For(thingDef, stuffDef);

            if (stat.Worker.ShouldShowFor(statReq) == true)
            {
                return stat.Worker.GetStatDrawEntryLabel(
                    stat,
                    stat.Worker.GetValue(statReq),
                    ToStringNumberSense.Absolute,
                    statReq
                );
            }
        }

        return null;
    }
    internal override ICellWidget? GetCellWidget(ThingDef thingDef, ThingDef? stuffDef)
    {
        var key = (thingDef, stuffDef);
        var exists = Cells.TryGetValue(key, out var cell);

        if (exists == false)
        {
            var value = GetValue(thingDef, stuffDef);

            if (value?.Length > 0)
            {
                cell = new(value);
            }

            Cells[key] = cell;
        }

        return cell;
    }
}
