using System;

namespace Stats;

internal abstract class Widget_TableCell
    : Widget
{
    public Widget_Table.ColumnProps Column { get; }
    public Widget_TableCell(
        Widget_Table.ColumnProps column
    )
    {
        Column = column;
    }
}
