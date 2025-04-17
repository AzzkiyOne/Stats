using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace Stats;

public class ColumnWorker_EquippedStatOffsets
    : ColumnWorker_Num
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.String;
    protected override float GetValue(ThingRec thing)
    {
        return thing.Def.equippedStatOffsets?.Count ?? 0f;
    }
    protected override IWidget GetTableCellContent(float value, ThingRec thing)
    {
        var labels = new StringBuilder();
        var values = new StringBuilder();

        foreach (var offset in thing.Def.equippedStatOffsets)
        {
            var offsetLabel = $"{offset.stat.LabelCap}:";
            var offsetValueStr = offset.stat.Worker.ValueToString(
                offset.value,
                true,
                ToStringNumberSense.Offset
            );

            labels.AppendInNewLine(offsetLabel);
            values.AppendInNewLine(offsetValueStr);
        }

        IWidget leftCol = new Widget_Label(labels.ToString());
        new WidgetComp_TextAnchor(ref leftCol, TextAnchor.LowerLeft);

        IWidget rightCol = new Widget_Label(values.ToString());
        new WidgetComp_Width_Rel(ref rightCol, 1f);
        new WidgetComp_TextAnchor(ref rightCol, TextAnchor.LowerRight);

        return new Widget_Container_Hor([leftCol, rightCol], 10f, true);
    }
}
