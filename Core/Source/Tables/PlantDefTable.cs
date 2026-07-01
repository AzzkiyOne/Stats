using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Verse;

namespace Stats.Tables;

public static class PlantDefTable
{
    public static MainTabWindowTab Make(TableDef tableDef)
    {
        List<PlantDefTableRecord> records = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            PlantProperties? plantProperties = thingDef.plant;

            if (thingDef.IsPlant && plantProperties is { isStump: false })
            {
                PlantDefTableRecord record = new(thingDef, plantProperties);
                records.Add(record);
            }
        }

        return new TableTab<PlantDefTableRecord>(tableDef, records);
    }
}
