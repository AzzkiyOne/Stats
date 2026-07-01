using System.Collections.Generic;
using Stats.TableRecords;
using Verse;

namespace Stats.Tables;

public static class AnimalDefTable
{
    public static MainTabWindowTab Make(TableDef tableDef)
    {
        List<PawnDefTableRecord> records = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            RaceProperties? raceProperties = thingDef.race;

            if (raceProperties != null && raceProperties.Animal && thingDef.IsCorpse == false)
            {
                PawnDefTableRecord record = new(thingDef, raceProperties);
                records.Add(record);
            }
        }

        return new TableTab<PawnDefTableRecord>(tableDef, records);
    }
}
