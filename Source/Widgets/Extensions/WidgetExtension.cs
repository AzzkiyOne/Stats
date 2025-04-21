namespace Stats.Widgets.Extensions;

public abstract class WidgetExtension
    : WidgetDecorator
{
    public override IWidget Widget { get; }
    public WidgetExtension(IWidget widget)
    {
        Widget = widget;
    }
}
