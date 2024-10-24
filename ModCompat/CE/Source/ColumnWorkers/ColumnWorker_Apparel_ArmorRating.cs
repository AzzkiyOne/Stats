using RimWorld;
using Verse;

namespace Stats.Compat.CE;

// This is simply a ColumnWorker_Num that uses GetStatDrawEntryLabel instead of
// ValueToString and adds an explanation.
//
// This case is somewhat similar to Caliber worker in that the only difference from a
// base worker is that it adds an explanation.
// 
// TODO:
// - Investigate a possibility of handling similar cases using XML-based API.
// OR
// - Make cell's "Explanation" property public and mutable. This of course is the
// easiest way of solving the problem, but probably not the most correct. If we'll go
// this route the "Explanation" property needs to be defined on the ICellWidget
// interface. Which means that any cell will have to have an explanation. On a second
// thought, this actually may be not that unreasonable of a requirement.
public class ColumnWorker_Apparel_ArmorRating : ColumnWorker_Num
{
    protected override ICellWidget<float>? CreateCell(ThingRec thing)
    {
        var value = GetValue(thing);

        if (value != 0f && float.IsNaN(value) == false)
        {
            var statReq = StatRequest.For(thing.Def, thing.StuffDef);
            var valueStr = ColumnDef.stat!.Worker.GetStatDrawEntryLabel(
                ColumnDef.stat!,
                value,
                ToStringNumberSense.Absolute,
                statReq
            );
            var valueExpl = ColumnDef.stat!.Worker.GetExplanationFinalizePart(statReq, ToStringNumberSense.Absolute, value);

            return new CellWidget_Num(value, valueStr, valueExpl);
        }

        return null;
    }
}
