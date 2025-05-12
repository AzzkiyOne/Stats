using UnityEngine;
using Verse;

namespace Stats.Compat.Biotech.GeneTable;

[StaticConstructorOnStartup]
public sealed class MetabolicEfficiencyColumnWorker : NumberColumnWorker<GeneDef>
{
    private static readonly Texture2D MetabolismIcon;
    static MetabolicEfficiencyColumnWorker()
    {
        MetabolismIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/Metabolism");
    }
    public MetabolicEfficiencyColumnWorker(ColumnDef columnDef) : base(columnDef, false, MetabolismIcon)
    {
    }
    protected override decimal GetValue(GeneDef geneDef)
    {
        return geneDef.biostatMet;
    }
}
