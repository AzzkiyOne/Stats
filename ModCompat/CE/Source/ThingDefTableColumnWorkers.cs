using System;
using System.Linq;
using CombatExtended;
using RimWorld;
using Stats.ThingDefTable;
using Verse;

namespace Stats.Compat.CE;

public class ColumnWorker_Caliber : StatColumnWorker
{
    protected override StatDef Stat => DefDatabase<StatDef>.GetNamed("Caliber");
    private ThingDef? GunDef(StatRequest req)
    {
        var def = req.Def as ThingDef;

        if (def?.building?.IsTurret ?? false)
        {
            def = def.building.turretGunDef;
        }

        return def;
    }
    public override DefReference? GetDefRef(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);
        var ammoSet = GunDef(statReq)?.GetCompProperties<CompProperties_AmmoUser>()?.ammoSet;
        var firstAmmoDef = ammoSet?.ammoTypes.FirstOrFallback()?.ammo;

        if (firstAmmoDef != null)
        {
            return new DefReference(firstAmmoDef);
        }

        return new DefReference(thing.Def, thing.Stuff);
    }
    public override string GetCellTip(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        return Stat.Worker.GetExplanationFull(
            statReq,
            ToStringNumberSense.Absolute,
            Stat.Worker.GetValue(statReq)
        );
    }
    public override IComparable GetCellSortValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);
        var gunDef = GunDef(statReq);
        var ammoSet = gunDef?.GetCompProperties<CompProperties_AmmoUser>()?.ammoSet;

        // This is pretty accurate.
        // TODO: figure out how to get the rest of guns/calibers to compute.
        if (gunDef != null && ammoSet != null)
        {
            return ammoSet.ammoTypes.Max(ammoLink =>
            {
                var projectileProps = ammoLink.projectile.projectile as ProjectilePropertiesCE;

                if (projectileProps.damageDef == DamageDefOf.Extinguish)
                {
                    return 0;
                }

                var dmg1 = projectileProps.GetDamageAmount(gunDef, gunDef.defaultStuff);
                var dmg2 = projectileProps.secondaryDamage.FirstOrFallback()?.amount ?? 1;

                return dmg1 * dmg2 * projectileProps.damageDef.buildingDamageFactorImpassable;
            });
        }

        return float.NaN;
    }
}

public class ColumnWorker_ReloadTime : StatColumnWorker
{
    public override bool ShouldShowFor(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.Stuff);

        return CE_StatDefOf.MagazineCapacity.Worker.ShouldShowFor(statReq);
    }
}