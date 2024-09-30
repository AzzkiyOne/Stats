using System;
using RimWorld;

namespace Stats.Table.Columns;

public sealed class Column_Num : Column
{
    public Func<ThingAlike, float?>? prop;
    public string formatString = "";
    public Column_Num() : base(ColumnStyle.Number) { }
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
    internal override ICell? GetCell(ThingAlike thing)
    {
        if (GetValue(thing) is float value && float.IsFinite(value))
        {
            return new Cells.Cell_Num(value, FormatValue(value));
        }

        return null;
    }
}
