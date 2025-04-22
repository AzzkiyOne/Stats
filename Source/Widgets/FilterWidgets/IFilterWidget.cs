namespace Stats.Widgets.FilterWidgets;

public interface IFilterWidget
    : IWidget
{
    IFilterExpression FilterExpression { get; }
    IFilterWidget Clone();
}
