using UnityEngine;

namespace Stats;

// TODO:
// - Investigate functional composition instead.

// There are 2 cases in which a widget can be drawn.
//
// 1. We know the size of parent's content box.
// 
// In this case, a widget is drawn as usual.
//
// 2. We don't know the size of parent's content box.
//
// For example, we want to draw a popover, which contains our widget, but which is not
// bigger/smaller than the widget.
//
// First, we need to measure the widget. Same as we would measure text.
// Then, we pass the measured size to the widget and draw it as usual.

// Box model is implemented as "box-sizing: border-box".
public abstract class Widget
{
    public abstract WidgetStyle Style { get; }
    public abstract Vector2 GetSize(in Vector2 containerSize);
    public abstract Vector2 GetSize();
    public abstract void Draw(Rect rect);
    public void DrawIn(Rect container)
    {
        Draw(new Rect(container.position, GetSize(container.size)));
    }
}
