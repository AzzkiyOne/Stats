﻿using RimWorld;

namespace Stats.ThingTable;

public class StatColumnDef : ColumnDef
{
#pragma warning disable CS8618
    public StatDef stat;
#pragma warning restore CS8618
    public StatValueExplanationType statValueExplanationType;
    public override void ResolveReferences()
    {
        if (string.IsNullOrEmpty(label))
        {
            label = stat.label;
        }

        if (string.IsNullOrEmpty(description))
        {
            description = stat.description;
        }

        base.ResolveReferences();
    }
}
