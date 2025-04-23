using UnityEngine;

namespace Stats.Widgets;

public abstract class WidgetBase : Widget
{
    protected abstract Vector2 Size { get; set; }
    public sealed override Vector2 GetSize(Vector2 containerSize)
    {
        return Size;
    }
    public sealed override void Draw(Rect rect, Vector2 containerSize)
    {
        GUIDebugger.DebugRect(this, rect);
        DrawContent(rect);
    }
    protected abstract void DrawContent(Rect rect);
    public sealed override void UpdateSize()
    {
        Size = GetSize();

        Parent?.UpdateSize();
    }
}
