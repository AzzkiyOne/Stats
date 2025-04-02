using UnityEngine;

namespace Stats;

public class Widget_Empty
    : Widget
{
    public override Vector2 AbsSize { get; } = Vector2.zero;
    public Widget_Empty()
    {
    }
}
