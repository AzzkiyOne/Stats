using UnityEngine;

namespace Stats.Widgets;

public abstract class Widget
{
    // Extensions doesn't count as parents.
    public virtual Widget? Parent { protected get; set; }
    private Vector2 SizeCached;
    private Vector2 ContainerSizeCached = Vector2.positiveInfinity;
    /*
    
    This method is used to calculate widget's size relative to its container size.

    The only argument is the size of its parent container. It is used to calculate relative dimensions of a widget's box.

    var containerSize = new Vector2(100f, 100f);

    var widget = new ExampleWidget()
        .SizeRel(0.5f, 0.25f);

    widget.GetSize(containerSize);// (50, 25)

    */
    public abstract Vector2 GetSize(Vector2 containerSize);
    public abstract Vector2 GetSize();
    public abstract void Draw(Rect rect, Vector2 containerSize);
    // Size caching actually gives very noticeable performance gain.
    //
    // Why not to cache size in GetSize?
    //
    // - In an event of cache invalidation it will cause cascading cache updates.
    // - GetSize can be overriden. Size extensions for example, rely on this.
    // - There is 2 of them.
    //
    // First 2 issues can be solved by implementing GetSize as Draw(Rect).
    // The third one, probably can be solved with another cache.
    //
    // TODO: Think about it.
    public void Draw(Rect rect)
    {
        var rectSize = rect.size;

        if (ContainerSizeCached.x == rectSize.x && ContainerSizeCached.y == rectSize.y)
        {
            rect.size = SizeCached;
        }
        else
        {
            ContainerSizeCached = rectSize;
            rect.size = SizeCached = GetSize(rectSize);
        }

        Draw(rect, rectSize);
    }
    public abstract void UpdateSize();
}
