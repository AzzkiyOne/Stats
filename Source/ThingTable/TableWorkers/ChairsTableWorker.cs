using Verse;

namespace Stats.ThingTable;

public sealed class ChairsTableWorker : TableWorker
{
    public ChairsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.building?.isSittable == true;
    }
}
