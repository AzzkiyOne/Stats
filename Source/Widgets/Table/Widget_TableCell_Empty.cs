using UnityEngine;

namespace Stats;

internal sealed class Widget_TableCell_Empty
    : Widget_TableCell
{
    public override Vector2 AbsSize => Vector2.zero;
    public Widget_TableCell_Empty(Widget_Table.ColumnProps column)
        : base(column)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Vector2.zero;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
    }
}
