using UnityEngine;

namespace Stats.Widgets;

/*

Extensions that change widget's size affect it differently depending on the order of application.

var containerSize = new Vector2(50f, 50f);

--- CSS box-sizing: border-box ---

var widget = new ExampleWidget()
    .PadAbs(10f)
    .SizeAbs(100f);

widget.GetSize(containerSize);// (100, 100)

--- CSS box-sizing: content-box ---

var widget = new ExampleWidget()
    .SizeAbs(100f)
    .PadAbs(10f);

widget.GetSize(containerSize);// (110, 110)

*/

/*

One case that isn't supported is the following:

var label = new Label("Label")
    .HeightRel(0.5f);

var container = new VerticalContainer([label]);

var viewSize = new Vector2(100f, 100f);// Doesn't really matter.

// Text.LineHeight == 22f
var containerSize = container.GetSize(viewSize);// containerSize.y == 22f
container.Draw(containerSize, viewSize);// Child is drawn 11px tall.

In CSS, label will be drawn 22px tall. There, if a child has no base to calculate its relative dimension from, the child's "own" size will be used instead for respective axis.

Supporting this case can deoptimize all normal cases because it ultimately boils down to conditionally disabling any relative size extensions on a child widget. And if they are not used, then why to have them?

*/

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
