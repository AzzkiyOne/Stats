namespace Stats;

internal sealed class Widget_TableCell_Normal
    : Widget_Addon,
      IWidget_TableCell
{
    public Widget_Table.ColumnProps Column { get; }
    public Widget_TableCell_Normal(
        IWidget widget,
        Widget_Table.ColumnProps column
    )
        : base(widget)
    {
        Column = column;
    }
}
