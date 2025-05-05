using System.Collections.Generic;
using Stats.GeneTable.Defs;
using Verse;

namespace Stats.GeneTable.TableWorkers;

public sealed class GeneTableWorker : Stats.TableWorkers.TableWorker<GeneDef>
{
    public static GeneTableWorker Make(TableDef _) => new();
    public override IEnumerable<GeneDef> GetRecords()
    {
        return DefDatabase<GeneDef>.AllDefs;
    }
}
