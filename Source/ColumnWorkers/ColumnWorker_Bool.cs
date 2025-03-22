using Verse;

namespace Stats;

public abstract class ColumnWorker_Bool : ColumnWorker<bool>
{
    public override ColumnCellStyle CellStyle => ColumnCellStyle.Boolean;
    protected override bool ShouldShowValue(bool value)
    {
        return value == true;
    }
    protected override Widget GetTableCellContent(bool value, ThingRec thing)
    {
        return new Widget_Texture(Widgets.GetCheckboxTexture(value))
        {
            Width = Text.LineHeight,
            Height = Text.LineHeight,
            //Scale = 0.7f,
        };
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
