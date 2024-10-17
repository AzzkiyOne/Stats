using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats;

public sealed class ColumnDef_Num : ColumnDef
{
    public PropDelegate<float>? prop;
    public string formatString = "";
    private readonly Dictionary<
        (ThingDef thingDef, ThingDef? stuffDef),
        CellWidget_Num?
    > Cells = [];
    public ColumnDef_Num() : base(ColumnStyle.Number) { }
    private float GetValue(ThingDef thingDef, ThingDef? stuffDef)
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
                return stat.Worker.GetValue(statReq);
            }

        }

        return 0f;
    }
    private string FormatValue(float value)
    {
        if (stat != null && formatString == "")
        {
            return stat.Worker.ValueToString(value, true);
        }

        return value.ToString(formatString);
    }
    internal override ICellWidget? GetCellWidget(ThingDef thingDef, ThingDef? stuffDef)
    {
        var key = (thingDef, stuffDef);
        var exists = Cells.TryGetValue(key, out var cell);

        if (exists == false)
        {
            var value = GetValue(thingDef, stuffDef);

            if (value != 0f)
            {
                cell = new(value, FormatValue(value));
            }

            Cells[key] = cell;
        }

        return cell;
    }
}
