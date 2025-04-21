using UnityEngine;

namespace Stats.Widgets.Comps;

public class ScrollOverflow
    : WidgetComp
{
    private Vector2 ScrollPosition;
    private IScrollable WidgetScrollable => (IScrollable)Widget;
    public ScrollOverflow(ref IWidget widget)
        : base(ref widget)
    {
    }
    public override void Draw(Rect rect, in Vector2 containerSize)
    {
        // Note to myself: Do the same thing as in table widget. Only this time, you probably don't need to pass offset because it can be read from rect.
        // TODO: Test with padding.
        // TODO: Add caching.
        var contentSize = WidgetScrollable.CalcContentSize(rect.size);

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

    public interface IScrollable
        : IWidget
    {
        // TODO: Pass scroll position to child.
        Vector2 CalcContentSize(in Vector2 size);
    }
}
