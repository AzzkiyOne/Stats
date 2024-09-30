using System;
using RimWorld;
using Verse;

namespace Stats.Table.Columns;

public sealed class Column_Str : Column
{
    public Func<ThingAlike, string?>? prop;
    private string? GetValue(ThingAlike thing)
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

            return stat.Worker.GetStatDrawEntryLabel(
                stat,
                stat.Worker.GetValue(statReq),
                ToStringNumberSense.Absolute,
                statReq
            );
        }

        return null;
    }
    internal override ICell? GetCell(ThingAlike thing)
    {
        if (GetValue(thing) is { Length: > 0 } value)
        {
            return new Cells.Cell_Str(value);
        }

        return null;
    }
}
