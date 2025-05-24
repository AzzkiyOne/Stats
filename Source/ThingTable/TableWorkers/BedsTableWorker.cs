using Verse;

namespace Stats.ThingTable;

public sealed class BedsTableWorker : TableWorker
{
    public BedsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsBed;
    }
}
