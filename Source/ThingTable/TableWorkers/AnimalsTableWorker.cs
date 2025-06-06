using Verse;

namespace Stats.ThingTable;

public sealed class AnimalsTableWorker : TableWorker
{
    public AnimalsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef is { race.Animal: true, IsCorpse: false };
    }
}
