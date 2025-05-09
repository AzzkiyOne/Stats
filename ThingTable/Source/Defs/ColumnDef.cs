using System;
using RimWorld;
using Verse;

namespace Stats.ThingTable;

public sealed class ColumnDef : Stats.ColumnDef, IColumnDef<ThingAlike>
{
    public StatDef? stat;
    public StatValueExplanationType statValueExplanationType;
#pragma warning disable CS8618
    public Func<ColumnDef, ColumnWorker<ThingAlike>> workerFactory;
    public ColumnWorker<ThingAlike> Worker { get; private set; }
#pragma warning restore CS8618
    public override void ResolveReferences()
    {
        if (stat != null)
        {
            if (string.IsNullOrEmpty(label))
            {
                label = stat.label;
            }

            if (string.IsNullOrEmpty(description))
            {
                description = stat.description;
            }
        }

        base.ResolveReferences();

        LongEventHandler.ExecuteWhenFinished(() => Worker = workerFactory(this));
    }
}
