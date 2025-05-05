using System;
using UnityEngine;
using Verse;

namespace Stats.GeneTable;

[StaticConstructorOnStartup]
public static class ComplexityColumnWorker
{
    private static readonly Texture2D ComplexityIcon;
    static ComplexityColumnWorker()
    {
        ComplexityIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/Complexity");
    }
    public static NumberColumnWorker<GeneDef> Make(ColumnDef _) => new(GetValue, ComplexityIcon);
    private static readonly Func<GeneDef, decimal> GetValue = geneDef =>
    {
        return geneDef.biostatCpx;
    };
}
