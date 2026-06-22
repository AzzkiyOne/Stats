using Verse;

namespace Stats.TableWorkers.ThingDef;

public sealed class AnimalTableWorker :
    PawnDefTableWorker
{
    public AnimalTableWorker(TableDef tableDef) : base(tableDef)
    {
    }

    protected override bool IsValidThingDef(Verse.ThingDef thingDef, RaceProperties raceProperties)
    {
        return raceProperties.Animal && thingDef.IsCorpse == false;
    }
}
