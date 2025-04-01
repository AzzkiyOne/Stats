using UnityEngine;
using Verse;

namespace Stats;

public abstract class Widget
    : IWidget
{
    protected abstract Vector2 ContentSize { get; }
    public Vector2 GetSize(in Vector2 containerSize)
    {
        return ContentSize;
    }
    public Vector2 GetSize()
    {
        return ContentSize;
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
