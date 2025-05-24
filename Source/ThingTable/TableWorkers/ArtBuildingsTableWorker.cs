using Verse;

namespace Stats.ThingTable;

public sealed class ArtBuildingsTableWorker : TableWorker
{
    public ArtBuildingsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.IsArt;
    }
}
