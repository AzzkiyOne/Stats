using Stats.Widgets;

namespace Stats;

public static class ColumnLabelFormat
{
    public static Widget LabelOnly(ColumnDef columnDef, ColumnCellStyle cellStyle)
    {
        return new Label(columnDef.LabelShort);
    }
    public static Widget IconOnly(ColumnDef columnDef, ColumnCellStyle cellStyle)
    {
        var icon = new InlineTexture(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor);

        return cellStyle switch
        {
            ColumnCellStyle.Number => new SingleElementContainer(icon.PaddingRel(1f, 0f, 0f, 0f)),
            ColumnCellStyle.Boolean => new SingleElementContainer(icon.PaddingRel(0.5f, 0f)),
            _ => icon
        };
    }
    public static Widget LabelWithIcon(ColumnDef columnDef, ColumnCellStyle cellStyle)
    {
        var label = new Label(columnDef.LabelShort);
        var icon = new InlineTexture(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor);

        return cellStyle switch
        {
            ColumnCellStyle.Number => new HorizontalContainer(
                [icon.PaddingRel(1f, 0f, 0f, 0f), label],
                Globals.GUI.PadSm,
                true
            ),
            ColumnCellStyle.Boolean => new SingleElementContainer(
                new HorizontalContainer([icon, label], Globals.GUI.PadSm).PaddingRel(0.5f, 0f)
            ),
            _ => new HorizontalContainer([icon, label], Globals.GUI.PadSm),
        };
    }
}
