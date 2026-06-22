using System.Collections.Generic;
using Stats.TableWorkers;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class GeneDefTableWorker : TableWorker<GeneDef>
{
    public override IEnumerable<GeneDef> InitialRecords => DefDatabase<GeneDef>.AllDefs;
    public GeneDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
