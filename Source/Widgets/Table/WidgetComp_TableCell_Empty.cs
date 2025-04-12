using UnityEngine;

namespace Stats;

internal sealed class WidgetComp_TableCell_Empty
    : WidgetComp_TableCell
{
    public override IWidget? Parent { set { } }
    public WidgetComp_TableCell_Empty(Widget_Table.Column column)
        : base(column)
    {
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Vector2.zero;
    }
    public override Vector2 GetSize()
    {
        return Vector2.zero;
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
    }
    public override void UpdateSize()
    {
    }
}
