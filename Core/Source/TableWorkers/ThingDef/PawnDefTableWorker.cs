using System;
using System.Collections.Generic;
using System.Linq;
using Stats.TableRecords;
using Verse;

namespace Stats.TableWorkers.ThingDef;

public abstract class PawnDefTableWorker :
    TableWorker<PawnDefTableRecord>,
    IRefRecordsProvider<Verse.ThingDef>,
    IRefRecordsProvider<Verse.Def>
{
    public override List<PawnDefTableRecord> InitialRecords { get; }
    IEnumerable<Verse.ThingDef> IRefRecordsProvider<Verse.ThingDef>.Records => InitialRecords.Select(record => record.ThingDef);
    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialRecords.Select(record => record.Def);

    public override event Action<PawnDefTableRecord>? OnRecordAdded;
    public override event Action<PawnDefTableRecord>? OnRecordRemoved;

    protected PawnDefTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<PawnDefTableRecord> initialRecords = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            RaceProperties? raceProperties = thingDef.race;

            if (raceProperties != null && IsValidThingDef(thingDef, raceProperties))
            {
                PawnDefTableRecord tableRecord = new(thingDef, raceProperties);
                initialRecords.Add(tableRecord);
            }
        }
        InitialRecords = initialRecords;
    }

    protected abstract bool IsValidThingDef(Verse.ThingDef thingDef, RaceProperties raceProperties);
}
