using UnityEngine;
using Verse;

namespace Stats.GeneTable;

[StaticConstructorOnStartup]
public sealed class MetabolicEfficiencyColumnWorker : NumberColumnWorker<GeneDef>
{
    private static readonly Texture2D MetabolismIcon;
    static MetabolicEfficiencyColumnWorker()
    {
        MetabolismIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/Metabolism");
    }
    public MetabolicEfficiencyColumnWorker() : base(false, MetabolismIcon)
    {
    }
    public static MetabolicEfficiencyColumnWorker Make(ColumnDef _) => new();
    protected override decimal GetValue(GeneDef geneDef)
    {
        return geneDef.biostatMet;
    }
}
