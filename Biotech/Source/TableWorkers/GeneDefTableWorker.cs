using System.Collections.Generic;
using Stats.Tables;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class GeneDefTableWorker : Table<GeneDef>
{
    public override IEnumerable<GeneDef> InitialRecords => DefDatabase<GeneDef>.AllDefs;
    public GeneDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
