using Verse;

namespace Stats;

public abstract class ColumnWorker_Bool
    : ColumnWorker<bool>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.Boolean;
    protected override bool ShouldShowValue(bool value)
    {
        return value == true;
    }
    protected override IWidget GetTableCellContent(bool value, ThingRec thing)
    {
        var style = new WidgetStyle()
        {
            Width = Text.LineHeight,
            Height = Text.LineHeight,
        };

        return
            new Widget_Container_Single(
                new Widget_Addon_Margin_Rel(
                    new Widget_Texture(Widgets.GetCheckboxTexture(value), style),
                    50, 0f
                )
            );
    }
    public override IWidget_FilterInput GetFilterWidget()
    {
        return new Widget_FilterInput_Bool(GetValue);
    }
    public override int Compare(ThingRec thing1, ThingRec thing2)
    {
        return GetValue(thing1).CompareTo(GetValue(thing2));
    }
}
