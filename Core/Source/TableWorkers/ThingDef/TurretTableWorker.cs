using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.TableWorkers.ThingDef;

public sealed class TurretTableWorker :
    TableWorker<TurretDefTableRecord>,
    IRefRecordsProvider<Verse.ThingDef>,
    IRefRecordsProvider<Verse.Def>
{
    public override List<TurretDefTableRecord> InitialRecords { get; }
    IEnumerable<Verse.ThingDef> IRefRecordsProvider<Verse.ThingDef>.Records => InitialRecords.Select(record => record.ThingDef);
    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialRecords.Select(record => record.Def);

    public override event Action<TurretDefTableRecord>? OnRecordAdded;
    public override event Action<TurretDefTableRecord>? OnRecordRemoved;

    public TurretTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<TurretDefTableRecord> initialRecords = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            BuildingProperties? buildingProperties = thingDef.building;
            VerbProperties? primaryVerbProperties = buildingProperties?.turretGunDef?.Verbs.Primary();

            if (primaryVerbProperties != null
                && buildingProperties is { IsTurret: true, turretGunDef: Verse.ThingDef turretGunDef }
                && thingDef.IsBuildingObtainableByPlayer())
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        TurretDefTableRecord tableRecord = new(thingDef, buildingProperties, turretGunDef, primaryVerbProperties, stuffDef);
                        initialRecords.Add(tableRecord);
                    }
                }
                else
                {
                    TurretDefTableRecord tableRecord = new(thingDef, buildingProperties, turretGunDef, primaryVerbProperties);
                    initialRecords.Add(tableRecord);
                }
            }
        }
        InitialRecords = initialRecords;
    }
}
