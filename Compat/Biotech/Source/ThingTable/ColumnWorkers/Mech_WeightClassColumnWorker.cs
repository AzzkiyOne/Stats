using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ThingTable;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.Biotech.ThingTable;

public sealed class Mech_WeightClassColumnWorker : ColumnWorker<ThingAlike>
{
    public Mech_WeightClassColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps == null)
        {
            return null;
        }

        return new Label(raceProps.mechWeightClass.ToStringHuman().CapitalizeFirst());
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var filterOptions = tableRecords
            .Select(thing => thing.Def.race?.mechWeightClass)
            .Distinct()
            .OrderBy(option => option)
            .Select<MechWeightClass?, NTMFilterOption<MechWeightClass?>>(
                mechWeightClass => mechWeightClass == null ? new() : new(mechWeightClass, ((MechWeightClass)mechWeightClass).ToStringHuman().CapitalizeFirst())
            );

        return Make.OTMFilter<ThingAlike, MechWeightClass?>(thing => thing.Def.race?.mechWeightClass, filterOptions);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Comparer<MechWeightClass?>.Default.Compare(thing1.Def.race?.mechWeightClass, thing2.Def.race?.mechWeightClass);
    }
}
