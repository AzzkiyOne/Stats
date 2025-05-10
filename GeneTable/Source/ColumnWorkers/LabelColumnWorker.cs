using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats.GeneTable;

public sealed class LabelColumnWorker : ColumnWorker<GeneDef>
{
    private LabelColumnWorker() : base(ColumnCellStyle.String)
    {
    }
    public static LabelColumnWorker Make(ColumnDef _) => new();
    public override Widget? GetTableCellWidget(GeneDef geneDef)
    {
        void openDefInfoDialog()
        {
            Draw.DefInfoDialog(geneDef);
        }

        return new HorizontalContainer(
            [
                new Icon(geneDef.Icon).ToButtonSubtle(openDefInfoDialog),
                new Label(geneDef.LabelCap),
            ],
            Globals.GUI.Pad
        )
        .Tooltip(geneDef.description);
    }
    public override FilterWidget<GeneDef> GetFilterWidget(IEnumerable<GeneDef> _)
    {
        return new StringFilter<GeneDef>(geneDef => geneDef.label);
    }
    public override int Compare(GeneDef geneDef1, GeneDef geneDef2)
    {
        return geneDef1.label.CompareTo(geneDef2.label);
    }
}
