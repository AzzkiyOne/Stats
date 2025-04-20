using RimWorld;
using Stats.ColumnWorkers.Generic;

namespace Stats.ColumnWorkers;

public class TechLevelColumnWorker
    : StringColumnWorker
{
    protected override string? GetValue(ThingAlike thing)
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
