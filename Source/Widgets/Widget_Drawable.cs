using UnityEngine;
using Verse;

namespace Stats;

public abstract class Widget_Drawable
    : IWidget
{
    public WidgetStyle Style { get; }
    protected abstract Vector2 ContentSize { get; }
    // Vector2.positiveInfinity is used instead of Vector2.zero
    // because there might be a case where passed container size
    // would be 0, but the widget size is not.
    // For example, when a widget width absolute dimensions is
    // rendered inside a flex container and takes all of the
    // available space.
    private Vector2 CurContainerSize = Vector2.positiveInfinity;
    private Vector2 CurSize = Vector2.zero;
    public Widget_Drawable(WidgetStyle? style = null)
    {
        Style = style ?? WidgetStyle.Default;
    }
    public Vector2 GetSize(in Vector2 containerSize)
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

        if (Style.Width != null)
        {
            CurSize.x = Style.Width.Get(containerSize.x);
        }
        else
        {
            CurSize.x = ContentSize.x;
        }

        if (Style.Height != null)
        {
            CurSize.y = Style.Height.Get(containerSize.y);
        }
        else
        {
            CurSize.y = ContentSize.y;
        }

        return CurSize;
    }
    public Vector2 GetSize()
    {
        Vector2 result;

        if (Style.Width is WidgetStyle.Units.Abs absWidth)
        {
            result.x = absWidth.Value;
        }
        else
        {
            result.x = ContentSize.x;
        }

        if (Style.Height is WidgetStyle.Units.Abs absHeight)
        {
            result.y = absHeight.Value;
        }
        else
        {
            result.y = ContentSize.y;
        }

        return result;
    }
    public virtual void Draw(Rect rect, in Vector2 containerSize)
    {
        // We can (could?) optimize here rendering in a scroll area.
        // If x/y coordinates are negative we can look if widget will be
        // visible.

        if (Mouse.IsOver(rect))
        {
            Widgets.DrawRectFast(rect, Color.cyan.ToTransparent(0.3f));
        }
    }
}
