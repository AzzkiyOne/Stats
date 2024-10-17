using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats;

public sealed class ColumnDef_Bool : ColumnDef
{
    public PropDelegate<bool>? prop;
    private readonly Dictionary<
        (ThingDef thingDef, ThingDef? stuffDef),
        CellWidget_Bool?
    > Cells = [];
    public ColumnDef_Bool() : base(ColumnStyle.Boolean) { }
    private bool GetValue(ThingDef thingDef, ThingDef? stuffDef)
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
                return stat.Worker.GetValue(statReq) > 0f;
            }
        }

        return false;
    }
    internal override ICellWidget? GetCellWidget(ThingDef thingDef, ThingDef? stuffDef)
    {
        var key = (thingDef, stuffDef);
        var exists = Cells.TryGetValue(key, out var cell);

        if (exists == false)
        {
            var value = GetValue(thingDef, stuffDef);

            if (value)
            {
                cell = new(value);
            }

            Cells[key] = cell;
        }

        return cell;
    }
}
