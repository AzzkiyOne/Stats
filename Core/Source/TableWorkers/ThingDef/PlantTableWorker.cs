using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Verse;

namespace Stats.TableWorkers.ThingDef;

public sealed class PlantTableWorker :
    TableWorker<PlantDefTableRecord>,
    IRefRecordsProvider<Verse.ThingDef>,
    IRefRecordsProvider<Verse.Def>
{
    public override List<PlantDefTableRecord> InitialRecords { get; }
    IEnumerable<Verse.ThingDef> IRefRecordsProvider<Verse.ThingDef>.Records => InitialRecords.Select(record => record.ThingDef);
    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialRecords.Select(record => record.Def);

    public override event Action<PlantDefTableRecord>? OnRecordAdded;
    public override event Action<PlantDefTableRecord>? OnRecordRemoved;

    public PlantTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<PlantDefTableRecord> initialRecords = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            PlantProperties? plantProperties = thingDef.plant;

            if (thingDef.IsPlant && plantProperties is { isStump: false })
            {
                PlantDefTableRecord tableRecord = new(thingDef, plantProperties);
                initialRecords.Add(tableRecord);
            }
        }
        InitialRecords = initialRecords;
    }
}
