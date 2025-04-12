namespace Stats;

public interface IWidget_FilterInput
    : IWidget
{
    IThingMatcher ThingMatcher { get; }
    IWidget_FilterInput Clone();
}
