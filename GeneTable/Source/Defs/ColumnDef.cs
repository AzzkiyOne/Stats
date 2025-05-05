using System;
using Verse;

namespace Stats.GeneTable;

public sealed class ColumnDef : Stats.ColumnDef, IColumnDef<GeneDef>
{
#pragma warning disable CS8618
    public Func<ColumnDef, ColumnWorker<GeneDef>> workerFactory;
    public ColumnWorker<GeneDef> Worker { get; private set; }
#pragma warning restore CS8618
    public override void ResolveReferences()
    {
        base.ResolveReferences();

        Worker = workerFactory(this);
    }
}
