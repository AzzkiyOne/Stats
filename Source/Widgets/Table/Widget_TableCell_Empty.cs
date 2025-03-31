using UnityEngine;

namespace Stats;

internal sealed class Widget_TableCell_Empty
    : IWidget_TableCell
{
    public Widget_Table.ColumnProps Column { get; }
    public WidgetStyle Style => WidgetStyle.Default;
    public Widget_TableCell_Empty(Widget_Table.ColumnProps column)
    {
        Column = column;
    }
    public Vector2 GetSize(in Vector2 containerSize)
    {
        return Vector2.zero;
    }
    public Vector2 GetSize()
    {
        return Vector2.zero;
    }
    public void Draw(Rect rect, in Vector2 containerSize)
    {
    }
}
