using RimWorld;

namespace Stats;

public class ColumnWorker_TechLevel : ColumnWorker_Str
{
    protected override string? GetValue(ThingRec thing)
    {
        return thing.Def.techLevel switch
        {
            TechLevel.Animal => "Animal",
            TechLevel.Neolithic => "Neolithic",
            TechLevel.Medieval => "Medieval",
            TechLevel.Industrial => "Industrial",
            TechLevel.Spacer => "Spacer",
            TechLevel.Ultra => "Ultra",
            TechLevel.Archotech => "Archotech",
            _ => "",
        };
    }
}
