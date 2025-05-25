using UnityEngine;

namespace Stats.Widgets.Extensions;

// Basic "overflow: auto" implementation.
// - Doesn't work in all cases.
// - Scroll optimizations in Horizontal/Vertical containers do not work.
public sealed class ScrollOverflowWidgetExtension : WidgetExtension
{
    private Vector2 ScrollPosition;
    internal ScrollOverflowWidgetExtension(Widget widget) : base(widget)
    {
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        // Note to myself: Do the same thing as in table widget. Only this time, you probably don't need to pass offset because it can be read from rect.
        var contentSize = Widget.GetSize(containerSize);

        if (contentSize.x > rect.width || contentSize.y > rect.height)
        {
            var viewRect = new Rect(Vector2.zero, contentSize);

            Verse.Widgets.BeginScrollView(rect, ref ScrollPosition, viewRect);

            Widget.Draw(viewRect, containerSize);

            Verse.Widgets.EndScrollView();
        }
        else
        {
            Widget.Draw(rect, containerSize);
        }
    }
}
