using Verse;

namespace Stats;

public class DefReference(Def def, ThingDef? stuffDef = null)
{
    public Def Def { get; } = def;
    public ThingDef? StuffDef { get; } = stuffDef;
}
