using System;
using CombatExtended;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.ThingTable.Defs;
using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ThingTable.Compat.CE.ColumnWorkers;

// TODO: Make a BooleanColumnWorker, just as NumberColumnWorker?
public sealed class RangedOneHandednessColumnWorker : ColumnWorker<ThingAlike>
{
    public static readonly Func<ThingAlike, bool> IsOneHandedWeapon =
        FunctionExtensions.Memoized((ThingAlike thing) =>
        {
            var statReq = StatRequest.For(thing.Def, thing.StuffDef);

            if (CE_StatDefOf.OneHandedness.Worker.ShouldShowFor(statReq))
            {
                return CE_StatDefOf.OneHandedness.Worker.GetValue(statReq) > 0f;
            }

            return false;
        });
    private RangedOneHandednessColumnWorker() : base(TableColumnCellStyle.Boolean)
    {
    }
    public static RangedOneHandednessColumnWorker Make(ColumnDef _) => new();
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
    public override FilterWidget<ThingAlike> GetFilterWidget()
    {
        return new BooleanFilter<ThingAlike>(IsOneHandedWeapon);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return IsOneHandedWeapon(thing1).CompareTo(IsOneHandedWeapon(thing2));
    }
}
