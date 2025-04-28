using Stats.Widgets;
using Stats.Widgets.FilterWidgets;

namespace Stats.ColumnWorkers;

public sealed class NameColumnWorker : ColumnWorker
{
    public override TableColumnCellStyle CellStyle => TableColumnCellStyle.String;
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        void openDefInfoDialog()
        {
            DefInfoDialog.Draw(thing.Def, thing.StuffDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thing).ToButtonSubtle(openDefInfoDialog),
                new Label(thing.Def.LabelCap),
            ],
            Globals.GUI.Pad
        )
        .Tooltip(thing.Def.description);
    }
    public override FilterWidget GetFilterWidget()
    {
        return new StringFilter(thing => thing.Def.label);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return thing1.Def.label.CompareTo(thing2.Def.label);
    }
}
