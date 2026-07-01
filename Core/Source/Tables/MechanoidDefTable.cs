using System.Collections.Generic;
using Stats.TableRecords;
using Verse;

namespace Stats.Tables;

public static class MechanoidDefTable
{
    public static MainTabWindowTab Make(TableDef tableDef)
    {
        List<PawnDefTableRecord> records = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            RaceProperties? raceProperties = thingDef.race;

            if (raceProperties != null && raceProperties.IsMechanoid && thingDef.IsCorpse == false)
            {
                PawnDefTableRecord tableRecord = new(thingDef, raceProperties);
                records.Add(tableRecord);
            }
        }

        return new TableTab<PawnDefTableRecord>(tableDef, records);
    }
}
