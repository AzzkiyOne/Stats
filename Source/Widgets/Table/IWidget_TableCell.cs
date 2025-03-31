namespace Stats;

internal interface IWidget_TableCell
    : IWidget
{
    Widget_Table.ColumnProps Column { get; }
}
