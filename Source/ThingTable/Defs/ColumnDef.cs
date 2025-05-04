using System;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.Defs;

namespace Stats.ThingTable.Defs;

public sealed class ColumnDef : Stats.Defs.ColumnDef, IColumnDef<ThingAlike>
{
    public StatDef? stat;
    public string? statValueFormatString;
    public StatValueExplanationType statValueExplanationType;
    // Indicates whether a value is "good" or "bad" in general.
    // Isn't used anywhere.
    public bool isNegative = false;
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

        Worker = workerFactory(this);
    }
}
