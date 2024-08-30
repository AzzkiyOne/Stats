using RimWorld;
using Verse;

namespace Stats;

[StaticConstructorOnStartup]
public static class StatsMod
{
    static StatsMod()
    {
        foreach (var statDef in DefDatabase<StatDef>.AllDefs)
        {
            var columnDef = new StatColumnDef();

            columnDef.defName = statDef.defName;

            DefGenerator.AddImpliedDef<ColumnDef>(columnDef, true);

            columnDef.ResolveReferences();
        }
    }
}
