using System.Collections.Generic;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Tables;

public static class MeleeWeaponDefTable
{
    public static MainTabWindowTab Make(TableDef tableDef)
    {
        List<ThingDefTableRecord> records = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            if (thingDef is { IsMeleeWeapon: true, destroyOnDrop: false })
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        ThingDefTableRecord record = new(thingDef, stuffDef);
                        records.Add(record);
                    }
                }
                else
                {
                    ThingDefTableRecord record = new(thingDef);
                    records.Add(record);
                }
            }
        }

        return new TableTab<ThingDefTableRecord>(tableDef, records);
    }
}
