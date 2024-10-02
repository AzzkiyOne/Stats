using System;
using RimWorld;

namespace Stats;

public sealed class ColumnDef_Num : ColumnDef
{
    public Func<ThingAlike, float?>? prop;
    public string formatString = "";
    public ColumnDef_Num() : base(ColumnStyle.Number) { }
    private float? GetValue(ThingAlike thing)
    {
        if (prop != null)
        {
            return prop(thing);
        }
        else if (stat != null)
        {
            var statReq = StatRequest.For(thing.Def, thing.Stuff);

            if (stat.Worker.ShouldShowFor(statReq) == false)
            {
                return null;
            }

            return stat.Worker.GetValue(statReq);
        }

        return null;
    }
    private string FormatValue(float value)
    {
        if (stat != null && formatString == "")
        {
            return stat.Worker.ValueToString(value, true);
        }

        return value.ToString(formatString);
    }
    internal override ICellWidget? GetCellWidget(ThingAlike thing)
    {
        if (GetValue(thing) is float value && float.IsFinite(value))
        {
            return new CellWidget_Num(value, FormatValue(value));
        }

        return null;
    }
}
