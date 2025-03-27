using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

public class ColumnWorker_EquippedStatOffsets
    : ColumnWorker_Num
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
    public override float GetValue(ThingRec thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? 0f;
    }
    protected override Widget GetTableCellContent(float value, ThingRec thing)
    {
        var col_left = new List<string>();
        var col_right = new List<string>();

        foreach (var offset in thing.Def.equippedStatOffsets)
        {
            var offsetLabel = $"{offset.stat.LabelCap}:";
            var offsetValueStr = offset.stat.Worker.ValueToString(
                offset.value,
                true,
                ToStringNumberSense.Offset
            );

            col_left.Add(offsetLabel);
            col_right.Add(offsetValueStr);
        }

        var leftColStyle = new WidgetStyle()
        {
            Width = null,
            TextAlign = TextAnchor.LowerLeft,
        };
        var rightColStyle = new WidgetStyle()
        {
            TextAlign = TextAnchor.LowerRight,
        };

        return new Widget_Container_Hor(
            [
                new Widget_Label(string.Join("\n", col_left),leftColStyle),
                new Widget_Label(string.Join("\n", col_right), rightColStyle),
            ],
            10f,
            true
        );
    }
}
