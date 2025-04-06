using UnityEngine;

namespace Stats;

/*

Comps that change widget's size affect it differently depending on the order of application.

var containerSize = new Vector2(50, 50);

--- CSS box-sizing: border-box ---

IWidget
w = new Widget_Example();
w = new WidgetComp_Size_Inc_Abs(w, 10);
w = new WidgetComp_Width_Abs(w, 100);

w.GetSize(containerSize).x;// 100

--- CSS box-sizing: content-box ---

IWidget
w = new Widget_Example();
w = new WidgetComp_Width_Abs(w, 100);
w = new WidgetComp_Size_Inc_Abs(w, 10);

w.GetSize(containerSize).x;// 110

*/

public interface IWidget
{
    IWidget? Parent { set; }
    /*
    
    These should only be set by size-constraining comps.

    They are mainly used by containers to resolve a situation when no size constraints were applied to the container and relative size constraints/modifiers were applied to its children. 
    
    In this case, a container should calculate its size on its own, by accumulating corresponding dimensions of its children. But at this point the container doesn't know its size, so it passes Vector2.positiveInfinity to its children's GetSize method as their container size, which makes them return their "own" size.

    IWidget
    label = new Widget_Label("Label");
    label = new WidgetComp_Height_Rel(label, 0.5f);
    var container = new Widget_Container_Ver([label]);

    var viewSize = new Vector2(100f, 100f);
    var containerSize = container.GetSize(viewSize);// containerSize.y == 22f
    var containerRect = new Rect(Vector2.zero, containerSize);

    container.Draw(containerRect, viewRect);

    The container is drawn exactly 22px tall, and label widget inside of it is also 22px tall. This is possible because container knows that it's size is unconstrained and assumes that the size of a rect passed to its Draw method was obtained by calling its GetSize method.

    You can look at vertical/horizontal containers for details.

    */
    bool WidthIsUndef { set; }
    bool HeightIsUndef { set; }
    /*
    
    This method is used to calculate widget's size relative to its container size and according to its box model.

    The only argument is the size of its parent container. It is used to calculate relative dimensions of a widget's box.

    var containerSize = new Vector2(100, 100);

    IWidget
    w = new Widget_Example();
    w = new WidgetComp_Size_Rel(w, 0.5, 0.25);

    w.GetSize(containerSize);// (50, 25)

    w = new WidgetComp_Size_Inc_Rel(w, 0.25);

    w.GetSize(containerSize);// (75, 50)

    If you want to get the sum of absolute size components, you can pass a Vector2.zero as container size.

    IWidget
    w = new Widget_Example();
    w = new WidgetComp_Size_Rel(w, 0.5, 0.25);
    w = new WidgetComp_Size_Inc_Abs(w, 10);

    w.GetSize(Vector2.zero);// (10, 10)

    This value doesn't include widget's "own" size.

    If container size is unknown use Vector2.positiveInfinity (or float.PositiveInfinity to mark dimension as undefined). This will ignore corresponding relative size constraints/modifiers and will also include its "own" size.

    IWidget
    w = new Widget_Label("FooBar");// "Own" height is 22f
    w = new WidgetComp_Size_Rel(w, 0.5, 0.75);

    var containerSize = new Vector2(100f, float.PositiveInfinity);

    w.GetSize(containerSize);// (50, 22)

    */
    Vector2 GetSize(in Vector2 containerSize);
    void Draw(Rect rect, in Vector2 containerSize);
    void UpdateSize();
}
