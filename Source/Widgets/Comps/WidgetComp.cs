using UnityEngine;

namespace Stats;

public abstract class WidgetComp
    : IWidget
{
    public IWidget? Parent { set => Widget.Parent = value; }
    public bool WidthIsUndef { set => Widget.WidthIsUndef = value; }
    public bool HeightIsUndef { set => Widget.HeightIsUndef = value; }
    protected IWidget Widget { get; }
    public WidgetComp(IWidget widget)
    {
        Widget = widget;
    }
    public virtual Vector2 GetSize(in Vector2 containerSize)
    {
        return Widget.GetSize(containerSize);
    }
    public virtual void Draw(Rect rect, in Vector2 containerSize)
    {
        Widget.Draw(rect, containerSize);
    }
    public void UpdateSize()
    {
        Widget.UpdateSize();
    }
}

public delegate float SizeFunc_SingleAxis(in Vector2 containerSize);
public delegate Vector2 SizeFunc_DoubleAxis(in Vector2 containerSize);
