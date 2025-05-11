using System.Collections.Generic;
using Verse;

namespace Stats.GeneTable;

public sealed class GeneTableWorker : TableWorker<GeneDef>
{
    public static GeneTableWorker Make(TableDef _) => new();
    public override IEnumerable<GeneDef> GetRecords()
    {
        return DefDatabase<GeneDef>.AllDefs;
    }
}
