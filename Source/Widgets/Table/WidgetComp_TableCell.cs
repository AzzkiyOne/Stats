using UnityEngine;

namespace Stats.Widgets.Table;

internal abstract class WidgetComp_TableCell
    : IWidget
{
    public abstract IWidget? Parent { set; }
    public GenericTable.Column Column;
    public WidgetComp_TableCell(GenericTable.Column column)
    {
        Column = column;
    }
    public abstract Vector2 GetSize(in Vector2 containerSize);
    public abstract Vector2 GetSize();
    public abstract void Draw(Rect rect, in Vector2 containerSize);
    public abstract void UpdateSize();
}
