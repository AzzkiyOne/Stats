using UnityEngine;
using Verse;

namespace Stats;

public abstract class Widget
    : IWidget
{
    public abstract Vector2 AbsSize { get; }
    public Vector2 GetSize(in Vector2 containerSize)
    {
        return AbsSize;
    }
    public virtual void Draw(Rect rect, in Vector2 containerSize)
    {
        if (Mouse.IsOver(rect))
        {
            Widgets.DrawRectFast(rect, Color.cyan.ToTransparent(0.3f));
        }
    }
}
