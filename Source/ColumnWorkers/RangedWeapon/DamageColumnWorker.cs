using System;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers.RangedWeapon;

public sealed class DamageColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Number;
    private static readonly Func<ThingAlike, int> GetValue = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
        {
            var verb = thing.Def.Verbs.Primary();
            var defaultProj = verb?.defaultProjectile?.projectile;

            if (defaultProj?.damageDef?.harmsHealth == true)
            {
                return defaultProj.GetDamageAmount(thing.Def, thing.StuffDef);
            }

            return default;
        }
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var value = GetValue(thing);

        if (value == default)
        {
            return null;
        }

        return new Label(value.ToString());
    }
    public override FilterWidget GetFilterWidget()
    {
        return new NumberFilter<int>(GetValue);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetValue(thing1).CompareTo(GetValue(thing2));
    }
}
