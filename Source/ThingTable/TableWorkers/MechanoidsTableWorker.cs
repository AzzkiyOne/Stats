using Verse;

namespace Stats.ThingTable;

public sealed class MechanoidsTableWorker : TableWorker
{
    public MechanoidsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.IsMechanoid: true, IsCorpse: false };
    }
}
