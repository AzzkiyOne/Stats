using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_Table_Things
    : Widget_Table
{
    private ColumnDef SortColumn = ColumnDefOf.Name;
    private SortDirection SortDirection = SortDirection.Ascending;
    public Widget_Table_Things(TableDef tableDef) : base()
    {
        List<ColumnDef> columns = [ColumnDefOf.Name, .. tableDef.columns];

        var headerRow = new Widget_TableRow_Header();

        foreach (ColumnDef column in columns)
        {
            var isPinned = column == ColumnDefOf.Name;
            var cell = new Widget_Cell_Header(column, this)
            {
                IsPinned = isPinned,
            };

            headerRow.Cells.Add(cell);
        }

        HeaderRows.Add(headerRow);

        var records = tableDef.Worker.GetRecords().ToList();

        records.Sort((r1, r2) =>
            SortColumn.Worker.Compare(r1, r2) * (int)SortDirection
        );

        for (int i = 0; i < records.Count; i++)
        {
            var rec = records[i];
            var row = new Widget_Row_Body()
            {
                Thing = rec,
            };

            for (int j = 0; j < columns.Count; j++)
            {
                var column = columns[j];
                var masterCell = headerRow.Cells[j];
                Widget_TableCell_Body cell;

                try
                {
                    var cellContent = column.Worker.GetTableCellContent(rec);
                    cell = new Widget_TableCell_Body(cellContent, masterCell);
                }
                catch
                {
                    cell = new Widget_TableCell_Body(null, masterCell);
                }

                row.Cells.Add(cell);
            }

            BodyRows.Add(row);
        }
    }
    private void HandleHeaderCellClick(ColumnDef column)
    {
        if (SortColumn == column)
        {
            SortDirection = (SortDirection)((int)SortDirection * -1);
            BodyRows.Reverse();
        }
        else
        {
            SortColumn = column;
            BodyRows.Sort((r1, r2) =>
                SortColumn.Worker.Compare(
                    ((Widget_Row_Body)r1).Thing,
                    ((Widget_Row_Body)r2).Thing
                ) * (int)SortDirection
            );
        }
    }
    private class Widget_Row_Body
        : Widget_TableRow_Body
    {
        public required ThingRec Thing { get; init; }
    }
    private class Widget_Cell_Header
        : IWidget_TableCell
    {
        public float MinWidth =>
            CellPadding * 2f
            +
            (
                Column.Icon != null
                    ? RowHeight
                    : Text.CalcSize(Column.LabelCap).x
            );
        public float Width { get; set; }
        public bool IsPinned { get; set; } = false;
        public ColumnDef Column { get; }
        private readonly Widget_Table_Things Parent;
        public Widget_Cell_Header(ColumnDef column, Widget_Table_Things parent)
        {
            Parent = parent;
            Column = column;
            Width = MinWidth;
        }
        public void Draw(Rect targetRect)
        {
            if (Parent.SortColumn == Column)
            {
                //Widgets.DrawBoxSolid(
                //    Parent.SortDirection == SortDirection.Ascending
                //        ? targetRect.BottomPartPixels(5f)
                //        : targetRect.TopPartPixels(5f),
                //    Color.yellow
                //);
                Widgets.DrawHighlightSelected(
                    Parent.SortDirection == SortDirection.Ascending
                        ? targetRect.BottomPartPixels(5f)
                        : targetRect.TopPartPixels(5f)
                );
            }

            var contentRect = targetRect.ContractedBy(CellPadding, 0f);

            if (Column.Icon != null)
            {
                contentRect = Column.Worker.CellStyle switch
                {
                    ColumnCellStyle.Number => contentRect.RightPartPixels(RowHeight),
                    ColumnCellStyle.Boolean => contentRect,
                    _ => contentRect.LeftPartPixels(RowHeight)
                };
                Widgets.DrawTextureFitted(contentRect, Column.Icon, 1f);
            }
            else
            {
                Text.Anchor = (TextAnchor)Column.Worker.CellStyle;
                Widgets.Label(contentRect, Column.LabelCap);
                Text.Anchor = Constants.DefaultTextAnchor;
            }

            TooltipHandler.TipRegion(targetRect, new TipSignal(Column.description));
            Widgets.DrawHighlightIfMouseover(targetRect);

            if (Widgets.ButtonInvisible(targetRect))
            {
                if (Event.current.control)
                {
                    IsPinned = !IsPinned;
                }
                else
                {
                    Parent.HandleHeaderCellClick(Column);
                }
            }
        }
    }
}
