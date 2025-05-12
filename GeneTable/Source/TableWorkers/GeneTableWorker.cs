using System.Collections.Generic;
using Verse;

namespace Stats.GeneTable;

public sealed class GeneTableWorker : TableWorker<GeneDef>
{
    protected override IEnumerable<GeneDef> Records => DefDatabase<GeneDef>.AllDefs;
    public GeneTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
