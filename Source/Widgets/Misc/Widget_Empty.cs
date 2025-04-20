using UnityEngine;

namespace Stats.Widgets.Misc;

public class Widget_Empty
    : Widget
{
    protected override Vector2 Size { get; set; }
    public Widget_Empty()
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
