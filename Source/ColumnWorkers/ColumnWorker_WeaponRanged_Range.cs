using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_Range : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        return thing.Def.Verbs.FirstOrFallback(v => v?.isPrimary == true)?.range ?? 0f;
    }
}
