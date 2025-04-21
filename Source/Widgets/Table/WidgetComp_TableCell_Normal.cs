using UnityEngine;
using Verse;

namespace Stats.Widgets.Table;

internal sealed class WidgetComp_TableCell_Normal
    : WidgetComp_TableCell
{
    public override IWidget? Parent { set => Widget.Parent = value; }
    private readonly IWidget Widget;
    private readonly TextAnchor TextAnchor;
    public WidgetComp_TableCell_Normal(
        IWidget widget,
        GenericTable.Column column,
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
        var origTextAnchor = Text.Anchor;
        Text.Anchor = TextAnchor;

        Widget.Draw(rect, containerSize);

        Text.Anchor = origTextAnchor;
    }
    public override void UpdateSize()
    {
        Widget.UpdateSize();
    }
}
