using UnityEngine;

namespace Stats;

internal abstract class WidgetComp_TableCell
    : IWidget
{
    public abstract bool WidthIsUndef { set; }
    public abstract bool HeightIsUndef { set; }
    public abstract IWidget? Parent { set; }
    public Widget_Table.ColumnProps Column;
    public WidgetComp_TableCell(Widget_Table.ColumnProps column)
    {
        Column = column;
    }
    public abstract Vector2 GetSize(in Vector2 containerSize);
    public abstract void Draw(Rect rect, in Vector2 containerSize);
    public abstract void UpdateSize();
}
