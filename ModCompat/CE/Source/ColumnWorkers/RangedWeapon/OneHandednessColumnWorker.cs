using System;
using CombatExtended;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ModCompat.CE.ColumnWorkers.RangedWeapon;

public sealed class OneHandednessColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.Boolean;
    public static readonly Func<ThingAlike, bool> IsOneHandedWeapon = FunctionExtensions.Memoized(
        (ThingAlike thing) =>
        {
            var statReq = StatRequest.For(thing.Def, thing.StuffDef);

            if (CE_StatDefOf.OneHandedness.Worker.ShouldShowFor(statReq))
            {
                return CE_StatDefOf.OneHandedness.Worker.GetValue(statReq) > 0f;
            }

            return false;
        }
    );
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var value = IsOneHandedWeapon(thing);

        if (value == false)
        {
            return null;
        }

        return new SingleElementContainer(
            new Icon(Verse.Widgets.CheckboxOnTex)
                .PaddingRel(0.5f, 0f)
        );
    }
    public override FilterWidget GetFilterWidget()
    {
        return new BooleanFilter(IsOneHandedWeapon);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return IsOneHandedWeapon(thing1).CompareTo(IsOneHandedWeapon(thing2));
    }
}
