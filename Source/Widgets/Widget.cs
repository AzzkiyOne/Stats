using UnityEngine;

namespace Stats.Widgets;

public abstract class Widget
{
    public Widget? Parent { protected get; set; }
    private Vector2 RelSizeCache;
    private Vector2 ContainerSizeCache = Vector2.positiveInfinity;
    private Vector2 AbsSizeCache;
    private bool AbsSizeCacheIsValid = false;
    /*
    
    This method is used to calculate widget's size relative to its container size.

    The only argument is the size of its parent container. It is used to calculate relative dimensions of a widget's box.

    var containerSize = new Vector2(100f, 100f);

    var widget = new ExampleWidget()
        .SizeRel(0.5f, 0.25f);

    widget.GetSize(containerSize);// (50, 25)

    */
    protected virtual Vector2 CalcSize(Vector2 containerSize)
    {
        return GetSize();
    }
    public Vector2 GetSize(Vector2 containerSize)
    {
        // I don't think we have to use Mathf.Approximately() here. Once everything settles down and
        // GUI becomes static, the same math should give the same results.
        if (ContainerSizeCache.x == containerSize.x && ContainerSizeCache.y == containerSize.y)
        {
            return RelSizeCache;
        }
        else
        {
            ContainerSizeCache = containerSize;

            return RelSizeCache = CalcSize(containerSize);
        }
    }
    // This method is used to calculate the "absolute" size of a widget,
    // ie. without any relative-size-related extensions.
    protected abstract Vector2 CalcSize();
    public Vector2 GetSize()
    {
        if (AbsSizeCacheIsValid)
        {
            return AbsSizeCache;
        }

        AbsSizeCacheIsValid = true;

        return AbsSizeCache = CalcSize();
    }
    public abstract void Draw(Rect rect, Vector2 containerSize);
    public void Resize()
    {
        ContainerSizeCache = Vector2.positiveInfinity;
        AbsSizeCache = CalcSize();
        AbsSizeCacheIsValid = true;// Just in case.

        Parent?.Resize();
    }
}
