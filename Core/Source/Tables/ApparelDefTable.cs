using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Tables;

// We do not check for "destroyOnDrop" for better compatibility with mods like
// VFE - Pirates.
public static class ApparelDefTable
{
    public static MainTabWindowTab Make(TableDef tableDef)
    {
        List<ApparelDefTableRecord> records = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            ApparelProperties? apparelProperties = thingDef.apparel;

            if (apparelProperties != null)
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        ApparelDefTableRecord record = new(thingDef, apparelProperties, stuffDef);
                        records.Add(record);
                    }
                }
                else
                {
                    ApparelDefTableRecord record = new(thingDef, apparelProperties);
                    records.Add(record);
                }
            }
        }

        return new TableTab<ApparelDefTableRecord>(tableDef, records);
    }
}
