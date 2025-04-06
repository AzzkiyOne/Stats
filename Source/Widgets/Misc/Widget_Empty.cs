using UnityEngine;

namespace Stats;

public class Widget_Empty
    : Widget
{
    public Widget_Empty()
    {
    }
    protected override Vector2 GetSize()
    {
        return Vector2.zero;
    }
    protected override void DrawContent(Rect rect)
    {
    }
}
