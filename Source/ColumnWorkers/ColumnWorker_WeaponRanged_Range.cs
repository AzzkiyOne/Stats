using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_Range : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        return thingDef.Verbs.FirstOrFallback(v => v?.isPrimary == true)?.range ?? 0f;
    }
}
