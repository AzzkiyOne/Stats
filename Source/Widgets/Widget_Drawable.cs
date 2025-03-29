using UnityEngine;
using Verse;

namespace Stats;

public abstract class Widget_Drawable
    : Widget
{
    public override WidgetStyle Style { get; }
    protected abstract Vector2 ContentSize { get; }
    private Vector2 CurContainerSize = Vector2.zero;
    private Vector2 CurSize = Vector2.zero;
    public Widget_Drawable(WidgetStyle? style = null)
    {
        Style = style ?? WidgetStyle.Default;
    }
    public override Vector2 GetSize(in Vector2 containerSize)
    {
        if
        (
            CurContainerSize.x == containerSize.x
            &&
            CurContainerSize.y == containerSize.y
        )
        {
            return CurSize;
        }

        CurContainerSize = containerSize;

        return CurSize = Style switch
        {
            {
                Width: not null,
                Height: not null
            } => new Vector2(
                Style.Width.Get(containerSize.x),
                Style.Height.Get(containerSize.y)
            ),
            {
                Width: null,
                Height: not null
            } => new Vector2(
                ContentSize.x,
                Style.Height.Get(containerSize.y)
            ),
            {
                Width: not null,
                Height: null
            } => new Vector2(
                Style.Width.Get(containerSize.x),
                ContentSize.y
            ),
            _ => ContentSize,
        };
    }
    public override Vector2 GetSize()
    {
        return Style switch
        {
            {
                Width: WidgetStyle.Units.Abs absWidth,
                Height: WidgetStyle.Units.Abs absHeight
            } => new Vector2(
                absWidth.Value,
                absHeight.Value
            ),
            {
                Width: null,
                Height: WidgetStyle.Units.Abs absHeight
            } => new Vector2(
                ContentSize.x,
                absHeight.Value
            ),
            {
                Width: WidgetStyle.Units.Abs absWidth,
                Height: null
            } => new Vector2(
                absWidth.Value,
                ContentSize.y
            ),
            _ => ContentSize,
        };
    }
    public override void Draw(Rect rect)
    {
        // We can (could?) optimize here rendering in a scroll area.
        // If x/y coordinates are negative we can look if the margin box will be
        // visible.

        if (Mouse.IsOver(rect))
        {
            Widgets.DrawRectFast(rect, Color.cyan.ToTransparent(0.3f));
        }

        Style.Background?.Invoke(rect, this);
    }
}
