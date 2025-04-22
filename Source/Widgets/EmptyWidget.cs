using UnityEngine;

namespace Stats.Widgets;

public sealed class EmptyWidget
    : Widget
{
    protected override Vector2 Size { get; set; }
    public EmptyWidget()
    {
        Size = GetSize();
    }
    public override Vector2 GetSize()
    {
        return Vector2.zero;
    }
    protected override void DrawContent(Rect rect)
    {
    }
}
