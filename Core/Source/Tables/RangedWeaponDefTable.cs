using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Tables;

public static class RangedWeaponDefTable
{
    public static MainTabWindowTab Make(TableDef tableDef)
    {
        List<RangedWeaponDefTableRecord> records = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            VerbProperties? primaryVerbProperties = thingDef.Verbs.Primary();

            if (primaryVerbProperties != null
                && thingDef is { IsRangedWeapon: true, destroyOnDrop: false }
                && thingDef.GetCompProperties<CompProperties_UniqueWeapon>() == null)
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        RangedWeaponDefTableRecord record = new(thingDef, primaryVerbProperties, stuffDef);
                        records.Add(record);
                    }
                }
                else
                {
                    RangedWeaponDefTableRecord record = new(thingDef, primaryVerbProperties);
                    records.Add(record);
                }
            }
        }

        return new TableTab<RangedWeaponDefTableRecord>(tableDef, records);
    }
}
