using UnityEngine;
using Verse;

namespace Stats.GeneTable;

[StaticConstructorOnStartup]
public sealed class ComplexityColumnWorker : NumberColumnWorker<GeneDef>
{
    private static readonly Texture2D ComplexityIcon;
    static ComplexityColumnWorker()
    {
        ComplexityIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/Complexity");
    }
    public ComplexityColumnWorker() : base(false, ComplexityIcon)
    {
    }
    public static ComplexityColumnWorker Make(ColumnDef _) => new();
    protected override decimal GetValue(GeneDef geneDef)
    {
        return geneDef.biostatCpx;
    }
}
