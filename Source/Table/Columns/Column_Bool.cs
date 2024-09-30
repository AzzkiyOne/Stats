using System;
using RimWorld;

namespace Stats.Table.Columns;

public sealed class Column_Bool : Column
{
    public Func<ThingAlike, bool?>? prop;
    public Column_Bool() : base(ColumnStyle.Boolean) { }
    private bool? GetValue(ThingAlike thing)
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

            return stat.Worker.GetValue(statReq) > 0f;
        }

        return null;
    }
    internal override ICell? GetCell(ThingAlike thing)
    {
        if (GetValue(thing) is bool value)
        {
            return new Cells.Cell_Bool(value);
        }

        return null;
    }
}
