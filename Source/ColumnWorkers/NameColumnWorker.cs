using Stats.ColumnWorkers.Generic;
using Stats.Widgets;
using Stats.Widgets.Containers;
using Stats.Widgets.Extensions;
using Stats.Widgets.Misc;
using Verse;

namespace Stats.ColumnWorkers;

public class NameColumnWorker
    : StringColumnWorker
{
    protected override string? GetValue(ThingAlike thing)
    {
        return thing.StuffDef == null
            ? thing.Def.LabelCap
            : $"{thing.Def.LabelCap} ({thing.StuffDef.LabelCap})";
    }
    protected override IWidget GetTableCellContent(string? value, ThingAlike thing)
    {
        void openDefInfoDialog()
        {
            DefInfoDialog.Draw(thing.Def, thing.StuffDef);
        }

        return new HorizontalContainer(
            [
                new ThingIcon(thing)
                    .HoverBackground(TexUI.HighlightTex)
                    .OnClick(openDefInfoDialog),
                new Label(value),
            ],
            10f
        ).Tooltip(thing.Def.description);
    }
}
