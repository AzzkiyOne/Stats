using System;
using UnityEngine;
using Verse;

namespace Stats.GeneTable;

[StaticConstructorOnStartup]
public static class RequiredArchiteCapsulesAmountColumnWorker
{
    private static readonly Texture2D ArchiteCapsuleIcon;
    static RequiredArchiteCapsulesAmountColumnWorker()
    {
        ArchiteCapsuleIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/ArchiteCapsuleRequired");
    }
    public static NumberColumnWorker<GeneDef> Make(ColumnDef _) => new(GetValue, ArchiteCapsuleIcon);
    private static readonly Func<GeneDef, decimal> GetValue = geneDef =>
    {
        return geneDef.biostatArc;
    };
}
