using System;
using UnityEngine;
using Verse;

namespace Stats.GeneTable;

[StaticConstructorOnStartup]
public static class MetabolicEfficiencyColumnWorker
{
    private static readonly Texture2D MetabolismIcon;
    static MetabolicEfficiencyColumnWorker()
    {
        MetabolismIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/Metabolism");
    }
    public static NumberColumnWorker<GeneDef> Make(ColumnDef _) => new(GetValue, MetabolismIcon);
    private static readonly Func<GeneDef, decimal> GetValue = geneDef =>
    {
        return geneDef.biostatMet;
    };
}
