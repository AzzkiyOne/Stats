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

            // hotReload = true so it won't add duplicates.
            DefGenerator.AddImpliedDef<ColumnDef>(columnDef, true);

            columnDef.ResolveReferences();
        }
    }
}
