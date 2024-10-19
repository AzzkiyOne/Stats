namespace Stats;

public class ColumnWorker_WeaponRanged_StoppingPower : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower ?? 0f;
    }
}
