using System.Text;
using Verse;

namespace Stats;

public class ColumnWorker_EquippedStatOffsets : ColumnWorker_Num
{
    public override float GetValue(ThingRec thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? 0f;
    }
    protected override IWidget GetTableCellContent(float value, ThingRec thing)
    {
        var tooltip = new StringBuilder();

        foreach (var offset in thing.Def.equippedStatOffsets)
        {
            var offsetValueStr = offset.stat.Worker.ValueToString(
                offset.value,
                true,
                ToStringNumberSense.Offset
            );

            tooltip.AppendLine($"{offset.stat.LabelCap}: {offsetValueStr}");
        }

        return new Widget_Label_Temp(
            FormatValue(value),
            tooltip.ToString()
        )
        { Style = CellStyle };
    }
}
