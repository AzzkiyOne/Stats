using UnityEngine;
using Verse;

namespace Stats.Compat.Biotech.GeneTable;

[StaticConstructorOnStartup]
public sealed class RequiredArchiteCapsulesAmountColumnWorker : NumberColumnWorker<GeneDef>
{
    private static readonly Texture2D ArchiteCapsuleIcon;
    static RequiredArchiteCapsulesAmountColumnWorker()
    {
        ArchiteCapsuleIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/ArchiteCapsuleRequired");
    }
    public RequiredArchiteCapsulesAmountColumnWorker(ColumnDef columnDef) : base(columnDef, false, ArchiteCapsuleIcon)
    {
    }
    protected override decimal GetValue(GeneDef geneDef)
    {
        return geneDef.biostatArc;
    }
}
