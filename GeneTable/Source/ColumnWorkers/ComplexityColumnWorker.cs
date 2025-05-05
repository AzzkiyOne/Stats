using System;
using Verse;

namespace Stats.GeneTable;

public static class ComplexityColumnWorker
{
    public static NumberColumnWorker<GeneDef> Make(ColumnDef _) => new(GetValue);
    private static readonly Func<GeneDef, decimal> GetValue = geneDef =>
    {
        return geneDef.biostatCpx;
    };
}
