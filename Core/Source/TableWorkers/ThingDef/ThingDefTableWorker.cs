using System;
using System.Collections.Generic;
using System.Linq;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.TableWorkers.ThingDef;

public abstract class ThingDefTableWorker :
    TableWorker<ThingDefTableRecord>,
    IRefRecordsProvider<Verse.ThingDef>,
    IRefRecordsProvider<Verse.Def>
{
    public override List<ThingDefTableRecord> InitialRecords { get; }
    IEnumerable<Verse.ThingDef> IRefRecordsProvider<Verse.ThingDef>.Records => InitialRecords.Select(record => record.ThingDef);
    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialRecords.Select(record => record.Def);

    public override event Action<ThingDefTableRecord>? OnRecordAdded;
    public override event Action<ThingDefTableRecord>? OnRecordRemoved;

    protected ThingDefTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<ThingDefTableRecord> initialRecords = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            if (IsValidThingDef(thingDef))
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        ThingDefTableRecord tableRecord = new(thingDef, stuffDef);
                        initialRecords.Add(tableRecord);
                    }
                }
                else
                {
                    ThingDefTableRecord tableRecord = new(thingDef);
                    initialRecords.Add(tableRecord);
                }
            }
        }
        InitialRecords = initialRecords;
    }

    protected abstract bool IsValidThingDef(Verse.ThingDef thingDef);
}
