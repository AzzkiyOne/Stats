namespace Stats.Widgets.Table.Filters.Widgets;

public interface IFilterWidget
    : IWidget
{
    IFilterExpression FilterExpression { get; }
    IFilterWidget Clone();
}
