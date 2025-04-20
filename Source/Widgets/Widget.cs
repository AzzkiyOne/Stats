using UnityEngine;

namespace Stats.Widgets;

public abstract class Widget
    : IWidget
{
    public IWidget? Parent { private get; set; }
    protected abstract Vector2 Size { get; set; }
    public Vector2 GetSize(in Vector2 containerSize)
    {
        return Size;
    }
    public abstract Vector2 GetSize();
    public void Draw(Rect rect, in Vector2 containerSize)
    {
        GUIDebugger.DebugRect(this, rect);
        DrawContent(rect);
    }
    protected abstract void DrawContent(Rect rect);
    public void UpdateSize()
    {
        Size = GetSize();

        Parent?.UpdateSize();
    }
}
