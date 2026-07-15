using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Tables;

public static class TurretDefTable
{
    public static MainTabWindowTab Make(TableDef tableDef)
    {
        List<TurretDefTableRecord> records = new(250);
        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            BuildingProperties? buildingProperties = thingDef.building;
            VerbProperties? primaryVerbProperties = buildingProperties?.turretGunDef?.Verbs.Primary();

            if (primaryVerbProperties != null
                && buildingProperties is { IsTurret: true, turretGunDef: ThingDef turretGunDef }
                && thingDef.IsBuildingObtainableByPlayer())
            {
                HashSet<ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (ThingDef stuffDef in stuffDefs)
                    {
                        TurretDefTableRecord record = new(thingDef, buildingProperties, turretGunDef, primaryVerbProperties, stuffDef);
                        records.Add(record);
                    }
                }
                else
                {
                    TurretDefTableRecord record = new(thingDef, buildingProperties, turretGunDef, primaryVerbProperties);
                    records.Add(record);
                }
            }
        }

        return new TableTab<TurretDefTableRecord>(tableDef, records);
    }
}
