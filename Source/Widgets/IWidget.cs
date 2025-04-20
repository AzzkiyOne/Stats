using UnityEngine;

namespace Stats.Widgets;

/*

Comps that change widget's size affect it differently depending on the order of application.

var containerSize = new Vector2(50f, 50f);

--- CSS box-sizing: border-box ---

IWidget w = new Widget_Example();
new WidgetComp_Size_Inc_Abs(ref w, 10f);
new WidgetComp_Size_Abs(ref w, 100f);

w.GetSize(containerSize);// (100, 100)

--- CSS box-sizing: content-box ---

IWidget w = new Widget_Example();
new WidgetComp_Size_Abs(ref w, 100f);
new WidgetComp_Size_Inc_Abs(ref w, 10f);

w.GetSize(containerSize);// (110, 110)

*/

/*

One case that isn't supported is the following:

IWidget label = new Widget_Label("Label");
new WidgetComp_Height_Rel(ref label, 0.5f);

IWidget container = new Widget_Container_Ver([label]);

var viewSize = new Vector2(100f, 100f);// Doesn't really matter.

// Text.LineHeight == 22f
var containerSize = container.GetSize(viewSize);// containerSize.y == 22f
container.Draw(containerSize, viewSize);// Child is drawn 11px tall.

In CSS, label will be drawn 22px tall. There, if a child has no base to calculate its relative dimension from, the child's "own" size will be used instead for respective axis.

Supporting this case can deoptimize all normal cases because it ultimately boils down to conditionally disabling any relative size components on a child widget. And if they are not used, then why to have them?

*/

public interface IWidget
{
    // Comps doesn't count as parents.
    IWidget? Parent { set; }
    /*
    
    This method is used to calculate widget's size relative to its container size.

    The only argument is the size of its parent container. It is used to calculate relative dimensions of a widget's box.

    var containerSize = new Vector2(100f, 100f);

    IWidget w = new Widget_Example();
    new WidgetComp_Size_Rel(ref w, 0.5f, 0.25f);

    w.GetSize(containerSize);// (50, 25)

    */
    Vector2 GetSize(in Vector2 containerSize);
    Vector2 GetSize();
    void Draw(Rect rect, in Vector2 containerSize);
    void UpdateSize();
}
