using Stats.ColumnWorkers.Generic;
using Stats.Widgets;

namespace Stats.ColumnWorkers;

public sealed class NameColumnWorker : StringColumnWorker
{
    protected override string? GetValue(ThingAlike thing)
    {
        return thing.StuffDef == null
            ? thing.Def.LabelCap
            : $"{thing.Def.LabelCap} ({thing.StuffDef.LabelCap})";
    }
    protected override Widget GetTableCellContent(string? value, ThingAlike thing)
    {
        void openDefInfoDialog()
        {
            DefInfoDialog.Draw(thing.Def, thing.StuffDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thing).ToButtonSubtle(openDefInfoDialog),
                new Label(value),
            ],
            Globals.UI.Pad
        ).Tooltip(thing.Def.description);
    }
}
