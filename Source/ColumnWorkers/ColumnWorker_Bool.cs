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
        var tex = Widgets.GetCheckboxTexture(value);
        IWidget
        icon = new Widget_Texture(tex);
        icon = new WidgetComp_Size_Abs(icon, Text.LineHeight);
        icon = new WidgetComp_Margin_Rel(icon, 0.5f, 0f);

        return new Widget_Container_Single(icon);
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
