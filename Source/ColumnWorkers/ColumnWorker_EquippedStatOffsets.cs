using System.Collections.Generic;
using Verse;

namespace Stats;

public class ColumnWorker_EquippedStatOffsets : ColumnWorker_Num
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
    public override float GetValue(ThingRec thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? 0f;
    }
    protected override Widget GetTableCellContent(float value, ThingRec thing)
    {
        var rows = new List<Widget>();

        foreach (var offset in thing.Def.equippedStatOffsets)
        {
            var offsetValueStr = offset.stat.Worker.ValueToString(
                offset.value,
                true,
                ToStringNumberSense.Offset
            );
            var widget = new Widget_Label($"{offset.stat.LabelCap}: {offsetValueStr}")
            {
                Width = 100,
            };

            rows.Add(widget);
        }

        return new Widget_Container_Ver(rows)
        {
            Width = 100,
        };
    }
}
