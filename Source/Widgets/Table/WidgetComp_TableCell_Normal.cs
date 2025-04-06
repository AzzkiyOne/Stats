using UnityEngine;
using Verse;

namespace Stats;

internal sealed class WidgetComp_TableCell_Normal
    : WidgetComp_TableCell
{
    public override bool WidthIsUndef { set => Widget.WidthIsUndef = value; }
    public override bool HeightIsUndef { set => Widget.WidthIsUndef = value; }
    public override IWidget? Parent { set => Widget.Parent = value; }
    private readonly IWidget Widget;
    private readonly TextAnchor TextAnchor;
    public WidgetComp_TableCell_Normal(
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
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        Text.Anchor = TextAnchor;

        Widget.Draw(rect, containerSize);

        Text.Anchor = Constants.DefaultTextAnchor;
    }
    public override void UpdateSize()
    {
        Widget.UpdateSize();
    }
}
