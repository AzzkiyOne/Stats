using System;
using RimWorld;

namespace Stats;

public sealed class ColumnDef_Bool : ColumnDef
{
    public Func<ThingAlike, bool?>? prop;
    public ColumnDef_Bool() : base(ColumnStyle.Boolean) { }
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
    internal override ICellWidget? GetCellWidget(ThingAlike thing)
    {
        if (GetValue(thing) is bool value)
        {
            return new CellWidget_Bool(value);
        }

        return null;
    }
}
