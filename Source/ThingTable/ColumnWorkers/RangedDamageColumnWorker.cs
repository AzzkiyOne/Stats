namespace Stats.ThingTable;

public sealed class RangedDamageColumnWorker : NumberColumnWorker<ThingAlike>
{
    public RangedDamageColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var verb = thing.Def.Verbs.Primary();
        var defaultProj = verb?.defaultProjectile?.projectile;

        if (defaultProj?.damageDef?.harmsHealth == true)
        {
            return defaultProj.GetDamageAmount(thing.Def, thing.StuffDef);
        }

        return 0m;
    }
}
