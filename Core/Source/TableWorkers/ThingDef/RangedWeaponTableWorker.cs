using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableRecords;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.TableWorkers.ThingDef;

public sealed class RangedWeaponTableWorker :
    TableWorker<RangedWeaponDefTableRecord>,
    IRefRecordsProvider<Verse.ThingDef>,
    IRefRecordsProvider<Verse.Def>
{
    public override List<RangedWeaponDefTableRecord> InitialRecords { get; }
    IEnumerable<Verse.ThingDef> IRefRecordsProvider<Verse.ThingDef>.Records => InitialRecords.Select(record => record.ThingDef);
    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialRecords.Select(record => record.Def);

    public override event Action<RangedWeaponDefTableRecord>? OnRecordAdded;
    public override event Action<RangedWeaponDefTableRecord>? OnRecordRemoved;

    public RangedWeaponTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<RangedWeaponDefTableRecord> initialRecords = new(250);
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
                        RangedWeaponDefTableRecord tableRecord = new(thingDef, primaryVerbProperties, stuffDef);
                        initialRecords.Add(tableRecord);
                    }
                }
                else
                {
                    RangedWeaponDefTableRecord tableRecord = new(thingDef, primaryVerbProperties);
                    initialRecords.Add(tableRecord);
                }
            }
        }
        InitialRecords = initialRecords;
    }
}
