using System;

namespace Stats.Table.Columns;

public sealed class Column_Thing : Column
{
    public Func<ThingAlike, ThingAlike?> prop = (ThingAlike thing) => thing;
    private ThingAlike? GetValue(ThingAlike thing)
    {
        return prop(thing);
    }
    internal override ICell? GetCell(ThingAlike thing)
    {
        if (GetValue(thing) is ThingAlike value)
        {
            return new Cells.Cell_Gen<string>(
                value.Label,
                value.Label,
                thing.Def.description,
                new ThingIcon(thing.Def, thing.Stuff),
                thing
            );
        }

        return null;
    }
}
