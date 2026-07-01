using System.Collections.Generic;
using RimWorld;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.Tables;

public static class ChairDefTable
{
    public static MainTabWindowTab Make(TableDef tableDef)
    {
        List<BuildingDefTableRecord> records = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            BuildingProperties? buildingProperties = thingDef.building;

            if (buildingProperties != null
                && thingDef.IsBuildingObtainableByPlayer()
                && buildingProperties.isSittable)
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        BuildingDefTableRecord record = new(thingDef, buildingProperties, stuffDef);
                        records.Add(record);
                    }
                }
                else
                {
                    BuildingDefTableRecord record = new(thingDef, buildingProperties);
                    records.Add(record);
                }
            }
        }

        return new TableTab<BuildingDefTableRecord>(tableDef, records);
    }
}
