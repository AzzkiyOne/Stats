using UnityEngine;

namespace Stats.Widgets;

public abstract class Widget
{
    // Extensions doesn't count as parents.
    public virtual Widget? Parent { protected get; set; }
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
    public abstract void UpdateSize();
}
