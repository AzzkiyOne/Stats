using UnityEngine;
using Verse;

namespace Stats;

internal sealed class Widget_TableCell_Normal
    : Widget_TableCell
{
    private readonly IWidget Widget;
    private readonly TextAnchor TextAnchor;
    public Widget_TableCell_Normal(
        IWidget widget,
        Widget_Table.ColumnProps column,
        ColumnCellStyle style
    )
        : base(column)
    {
        Widget = widget;
        TextAnchor = (TextAnchor)style;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public override Vector2 GetSize()
    {
        return Widget.GetSize();
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        Text.Anchor = TextAnchor;

        Widget.Draw(rect, containerSize);

        Text.Anchor = Constants.DefaultTextAnchor;
    }
}
