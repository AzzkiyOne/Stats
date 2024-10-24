using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class TableWidget_Main : TableWidget_Base
{
    private readonly TableWidget_Selected SelectedThingsTable;
    private bool DrawSelectedThingsTable = false;
    public TableWidget_Main(TableDef tableDef) : base(
        [ColumnDefOf.Name, .. tableDef.columns]
    )
    {
        Left = new TablePart_M(this, [ColumnDefOf.Name]);
        Right = new TablePart_M(this, tableDef.columns);
        SelectedThingsTable = new(Columns, Left.Columns, Right.Columns);

        InitRows(tableDef);
        // Can't move this to base class (for now) because it requires for rows to be
        // initialized.
        SyncLayout();
    }
    public override void Draw(Rect targetRect)
    {
        if (Event.current.type == EventType.KeyDown && Event.current.alt)
        {
            DrawSelectedThingsTable = !DrawSelectedThingsTable;
        }

        if (DrawSelectedThingsTable)
        {
            SelectedThingsTable.Draw(targetRect);
        }
        else
        {
            base.Draw(targetRect);
        }
    }
    protected override void HandleRowSelect(ThingRec row)
    {
        if (SelectedThingsTable.Contains(row))
        {
            SelectedThingsTable.RemoveRow(row);
        }
        else
        {
            SelectedThingsTable.AddRow(row);
        }
    }
    private class TablePart_M : TablePart
    {
        public TablePart_M(TableWidget_Main parent, List<ColumnDef> columns) :
            base(parent, columns)
        {
            ShouldDrawRowAddon = true;
        }
        protected override void DrawRowAddon(Rect rowRect, ThingRec row)
        {
            if (
                Event.current.type == EventType.Repaint
                && ((TableWidget_Main)Parent).SelectedThingsTable.Contains(row)
            )
            {
                Widgets.DrawHighlightSelected(rowRect);
            }
        }
    }
}
