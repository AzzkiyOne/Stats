using Verse;

namespace Stats.ThingTable;

public sealed class AnomalyEntitiesTableWorker : TableWorker
{
    public AnomalyEntitiesTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.IsAnomalyEntity: true, IsCorpse: false };
    }
}
