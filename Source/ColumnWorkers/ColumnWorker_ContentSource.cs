using UnityEngine;

namespace Stats;

public class ColumnWorker_ContentSource
    : ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.Def.modContentPack?.Name;
    }
    protected override Widget GetTableCellContent(string? value, ThingRec thing)
    {
        var style = new WidgetStyle()
        {
            TextAlign = (TextAnchor)CellStyle,
        };

        return new Widget_Label(value!, style)
        {
            Tooltip = thing.Def.modContentPack.PackageIdPlayerFacing,
        };
    }
}
