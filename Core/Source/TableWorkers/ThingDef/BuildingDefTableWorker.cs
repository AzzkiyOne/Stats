using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.TableWorkers.ThingDef;

public abstract class BuildingDefTableWorker :
    TableWorker<BuildingDefTableRecord>,
    IRefRecordsProvider<Verse.ThingDef>,
    IRefRecordsProvider<Verse.Def>
{
    public override List<BuildingDefTableRecord> InitialRecords { get; }
    IEnumerable<Verse.ThingDef> IRefRecordsProvider<Verse.ThingDef>.Records => InitialRecords.Select(record => record.ThingDef);
    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialRecords.Select(record => record.Def);

    public override event Action<BuildingDefTableRecord>? OnRecordAdded;
    public override event Action<BuildingDefTableRecord>? OnRecordRemoved;

    protected BuildingDefTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<BuildingDefTableRecord> initialRecords = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            BuildingProperties? buildingProperties = thingDef.building;
            if (buildingProperties != null
                && thingDef.IsBuildingObtainableByPlayer()
                && IsValidThingDef(thingDef, buildingProperties))
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        BuildingDefTableRecord tableRecord = new(thingDef, buildingProperties, stuffDef);
                        initialRecords.Add(tableRecord);
                    }
                }
                else
                {
                    BuildingDefTableRecord tableRecord = new(thingDef, buildingProperties);
                    initialRecords.Add(tableRecord);
                }
            }
        }
        InitialRecords = initialRecords;
    }

    protected abstract bool IsValidThingDef(Verse.ThingDef thingDef, BuildingProperties buildingProperties);
}
