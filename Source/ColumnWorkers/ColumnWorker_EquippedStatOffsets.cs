using System.Text;
using Verse;

namespace Stats;

public class ColumnWorker_EquippedStatOffsets : ColumnWorker_Num
{
    protected override float GetValue(ThingRec thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? 0f;
    }
    protected override ICellWidget<float>? CreateCell(ThingRec thing)
    {
        var value = GetValue(thing);

        if (value != 0f && float.IsNaN(value) == false)
        {
            var valueExpl = new StringBuilder();

            foreach (var offset in thing.Def.equippedStatOffsets)
            {
                var offsetValueStr = offset.stat.Worker.ValueToString(
                    offset.value,
                    true,
                    ToStringNumberSense.Offset
                );

                valueExpl.AppendLine($"{offset.stat.LabelCap}: {offsetValueStr}");
            }

            return new CellWidget_Num(value, value.ToString(), valueExpl.ToString());
        }

        return null;
    }
}
