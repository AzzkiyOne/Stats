﻿using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.Biotech.GeneTable;

public sealed class LabelColumnWorker : ColumnWorker<GeneDef>
{
    public LabelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    public override Widget? GetTableCellWidget(GeneDef geneDef)
    {
        void openDefInfoDialog()
        {
            Draw.DefInfoDialog(geneDef);
        }

        return new HorizontalContainer(
            [
                new Icon(geneDef.Icon).ToButtonGhostly(openDefInfoDialog),
                new Label(geneDef.LabelCap),
            ],
            Globals.GUI.Pad
        )
        .Tooltip(geneDef.description);
    }
    public override FilterWidget<GeneDef> GetFilterWidget(IEnumerable<GeneDef> _)
    {
        return Make.StringFilter((GeneDef geneDef) => geneDef.label);
    }
    public override int Compare(GeneDef geneDef1, GeneDef geneDef2)
    {
        return geneDef1.label.CompareTo(geneDef2.label);
    }
}
