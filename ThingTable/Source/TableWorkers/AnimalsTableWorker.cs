using Verse;

namespace Stats.ThingTable;

public sealed class AnimalsTableWorker : TableWorker
{
    public static AnimalsTableWorker Make(TableDef _) => new();
    protected override bool IsValidThingDef(ThingDef thingDef)
    {
        return thingDef.race?.Animal == true && thingDef.IsCorpse == false;
    }
}
