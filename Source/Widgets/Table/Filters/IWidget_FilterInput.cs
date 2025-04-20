namespace Stats.Widgets.Table.Filters;

public interface IWidget_FilterInput
    : IWidget
{
    IThingMatcher ThingMatcher { get; }
    IWidget_FilterInput Clone();
}
