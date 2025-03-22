namespace Stats;

public class ColumnWorker_WeaponRanged_Range : ColumnWorker_Num
{
    public override float GetValue(ThingRec thing)
    {
        return thing.Def.Verbs.Primary()?.range ?? 0f;
    }
}
