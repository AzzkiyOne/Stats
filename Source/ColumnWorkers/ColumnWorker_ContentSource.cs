namespace Stats;

public class ColumnWorker_ContentSource
    : ColumnWorker_Str
{
    public override string? GetValue(ThingRec thing)
    {
        return thing.Def.modContentPack?.Name;
    }
    protected override IWidget GetTableCellContent(string? value, ThingRec thing)
    {
        var tooltip = thing.Def.modContentPack.PackageIdPlayerFacing;
        IWidget
        widget = new Widget_Label(value!);
        widget = new WidgetComp_Tooltip(widget, tooltip);

        return widget;
    }
}
