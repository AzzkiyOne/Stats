using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.TableWorkers.ThingDef;

// We do not check for "destroyOnDrop" for better compatibility with mods like
// VFE - Pirates.
public sealed class ApparelTableWorker :
    TableWorker<ApparelDefTableRecord>,
    IRefRecordsProvider<Verse.ThingDef>,
    IRefRecordsProvider<Verse.Def>
{
    public override List<ApparelDefTableRecord> InitialRecords { get; }
    IEnumerable<Verse.ThingDef> IRefRecordsProvider<Verse.ThingDef>.Records => InitialRecords.Select(record => record.ThingDef);
    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialRecords.Select(record => record.Def);

    public override event Action<ApparelDefTableRecord>? OnRecordAdded;
    public override event Action<ApparelDefTableRecord>? OnRecordRemoved;

    public ApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<ApparelDefTableRecord> initialRecords = new(250);
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
                        ApparelDefTableRecord tableRecord = new(thingDef, apparelProperties, stuffDef);
                        initialRecords.Add(tableRecord);
                    }
                }
                else
                {
                    ApparelDefTableRecord tableRecord = new(thingDef, apparelProperties);
                    initialRecords.Add(tableRecord);
                }
            }
        }
        InitialRecords = initialRecords;
    }
}
