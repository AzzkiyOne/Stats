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
    protected override Widget GetTableCellContent(bool value, ThingRec thing)
    {
        var style = new WidgetStyle()
        {
            Width = Text.LineHeight,
            Height = Text.LineHeight,
        };

        return
            new Widget_Container_Hor([
                new Widget_Empty(new () { Width = 50 }),
                new Widget_Texture(Widgets.GetCheckboxTexture(value), style),
                new Widget_Empty(new () { Width = 50 }),
            ], flex: true);
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
