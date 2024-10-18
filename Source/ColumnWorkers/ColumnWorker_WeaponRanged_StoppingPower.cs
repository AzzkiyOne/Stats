using Verse;

namespace Stats;

public class ColumnWorker_WeaponRanged_StoppingPower : ColumnWorker_Num
{
    protected override float GetValue(ThingDef thingDef, ThingDef? stuffDef)
    {
        var verb = thingDef.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        return defaultProj?.stoppingPower ?? 0f;
    }
}
