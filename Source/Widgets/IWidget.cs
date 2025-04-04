using UnityEngine;

namespace Stats;

/*

There are 2 cases in which a widget can be drawn.

1. We know the size of parent's content box.


In this case, a widget is drawn as usual.

2. We don't know the size of parent's content box.

For example, we want to draw a popover, which contains our widget, but which is not
bigger/smaller than the widget.

First, we need to measure the widget. Same as we would measure text.

Then, we pass the measured size to the widget and draw it as usual.

*/

/*

Comps that change widget's size affect it differently depending on order of application.

var containerSize = new Vector2(50, 50);

--- CSS box-sizing: border-box ---

IWidget
w = new Widget_Example();
w = new Size_Inc_Abs(w, 10);
w = new Width_Abs(w, 100);

w.AbsSize.x;// 100
w.GetSize(containerSize).x;// 100

--- CSS box-sizing: content-box ---

IWidget
w = new Widget_Example();
w = new Width_Abs(w, 100);
w = new Size_Inc_Abs(w, 10);

w.AbsSize.x;// 110
w.GetSize(containerSize).x;// 110

*/

public interface IWidget
{
    /*

    AbsSize (absolute/abstract size) is the size of a widget as if it were given infinite space to draw. Since widget's container size is unknown, relative size constraints/modifiers have no effect.

    var containerSize = new Vector2(100, 100);

    IWidget
    w = new Widget_Example();
    w.AbsSize.x;// 50
    w.GetSize(containerSize).x;// 50

    --- Absolute size ---

    w = new Size_Inc_Abs(w, 10);
    w.AbsSize.x;// 60
    w.GetSize(containerSize).x;// 60

    w = new Width_Abs(w, 100);
    w.AbsSize.x;// 100
    w.GetSize(containerSize).x;// 100

    w = new Size_Inc_Abs(w, 10);
    w.AbsSize.x;// 110
    w.GetSize(containerSize).x;// 110

    --- Relative size ---

    w = new Size_Inc_Rel(w, 0.1);
    w.AbsSize.x;// 50
    w.GetSize(containerSize).x;// 60

    w = new Width_Rel(w, 1);
    w.AbsSize.x;// 50
    w.GetSize(containerSize).x;// 100

    w = new Size_Inc_Rel(w, 0.1);
    w.AbsSize.x;// 50
    w.GetSize(containerSize).x;// 110

    --- Mixed size ---

    w = new Size_Inc_Abs(w, 100);
    w.AbsSize.x;// 150
    w.GetSize(containerSize).x;// 150

    w = new Width_Rel(w, 1);
    w.AbsSize.x;// 150
    w.GetSize(containerSize).x;// 100

    w = new Size_Inc_Abs(w, 50);
    w.AbsSize.x;// 200
    w.GetSize(containerSize).x;// 150

    */
    Vector2 AbsSize { get; }
    /*
    
    This method is used to calculate widget's size relative to its container size and according to its box model.

    The only argument is the size of its parent container. It is used to calculate relative dimensions of a widget's box.

    var containerSize = new Vector2(100, 100);

    IWidget
    w = new Widget_Example();
    w = new Size_Rel(w, 0.5, 0.25);

    w.GetSize(containerSize);// (50, 25)

    w = new Size_Inc_Rel(w, 0.25);

    w.GetSize(containerSize);// (75, 50)

    If you want to get the sum of absolute size components, you can pass a Vector2.zero as container size.

    IWidget
    w = new Widget_Example();
    w = new Size_Rel(w, 0.5, 0.25);
    w = new Size_Inc_Abs(w, 10);

    w.GetSize(Vector2.zero);// (10, 10)

    This value doesn't include widget's own AbsSize.

    */
    Vector2 GetSize(in Vector2 containerSize);
    void Draw(Rect rect, in Vector2 containerSize);
}
