using UnityEngine;
using Verse;

namespace Stats.GeneTable;

[StaticConstructorOnStartup]
public sealed class RequiredArchiteCapsulesAmountColumnWorker : NumberColumnWorker<GeneDef>
{
    private static readonly Texture2D ArchiteCapsuleIcon;
    static RequiredArchiteCapsulesAmountColumnWorker()
    {
        ArchiteCapsuleIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/ArchiteCapsuleRequired");
    }
    public RequiredArchiteCapsulesAmountColumnWorker() : base(false, ArchiteCapsuleIcon)
    {
    }
    public static RequiredArchiteCapsulesAmountColumnWorker Make(ColumnDef _) => new();
    protected override decimal GetValue(GeneDef geneDef)
    {
        return geneDef.biostatArc;
    }
}
