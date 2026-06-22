using Verse;

namespace Stats.TableWorkers.ThingDef;

public sealed class MechanoidTableWorker :
    PawnDefTableWorker
{
    public MechanoidTableWorker(TableDef tableDef) : base(tableDef)
    {
    }

    protected override bool IsValidThingDef(Verse.ThingDef thingDef, RaceProperties raceProperties)
    {
        return raceProperties.IsMechanoid && thingDef.IsCorpse == false;
    }
}
