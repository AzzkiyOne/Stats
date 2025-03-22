using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class Widget_Table_Things
    : Widget_Table
{
    private ColumnDef SortColumn = ColumnDefOf.Name;
    private SortDirection SortDirection = SortDirection.Ascending;
    public Widget_Table_Things(TableDef tableDef)
        : base()
    {
        List<ColumnDef> columns = [ColumnDefOf.Name, .. tableDef.columns];
        var cellPadding = (15f, 15f, 5f, 5f);

        // Headers

        var headerRow = new Widget_TableRow()
        {
            Background = static (borderBox, _, _) =>
            {
                Widgets.DrawHighlight(borderBox);
                Widgets.DrawLineHorizontal(
                    borderBox.x,
                    borderBox.yMax - 1f,
                    borderBox.width,
                    StatsMainTabWindow.BorderLineColor
                );
            },
        };

        foreach (var column in columns)
        {
            Widget cellContent;

            if (column.Icon != null)
            {
                Widget.AlignFunc? align = column.Worker.CellStyle switch
                {
                    ColumnCellStyle.Number => Widget.Align.Right,
                    ColumnCellStyle.Boolean => Widget.Align.Middle_H,
                    _ => null,
                };

                cellContent = new Widget_Texture(column.Icon)
                {
                    Width = Text.LineHeight,
                    Height = Text.LineHeight,
                    Align_H = align,
                };
            }
            else
            {
                cellContent = new Widget_Label(column.LabelCap)
                {
                    Width = 100,
                };
            }

            var cell = new Widget_TableCell([cellContent])
            {
                Padding = cellPadding,
                Props = new Widget_TableCell.Properties()
                {
                    IsPinned = column == ColumnDefOf.Name,
                    TextAnchor = (TextAnchor)column.Worker.CellStyle,
                },
                Tooltip = column.description,
                Background = (borderBox, widget) =>
                {
                    Widgets.DrawHighlightIfMouseover(borderBox);

                    if (SortColumn == column)
                    {
                        Widgets.DrawBoxSolid(
                            SortDirection == SortDirection.Ascending
                                ? borderBox.BottomPartPixels(5f)
                                : borderBox.TopPartPixels(5f),
                            Color.yellow.ToTransparent(0.3f)
                        );
                    }

                    if (Widgets.ButtonInvisible(borderBox))
                    {
                        if (Event.current.control)
                        {
                            var cell = (Widget_TableCell)widget;

                            cell.Props.IsPinned = !cell.Props.IsPinned;
                        }
                        else
                        {
                            SortRowsByColumn(column);
                        }
                    }
                }
            };

            headerRow.AddCell(cell);
        }

        AddHeaderRow(headerRow);

        // Body

        var records = tableDef.Worker.GetRecords().ToList();

        records.Sort((r1, r2) =>
            SortColumn.Worker.Compare(r1, r2) * (int)SortDirection
        );

        for (int i = 0; i < records.Count; i++)
        {
            var rec = records[i];
            var row = new Widget_TableRow<ThingRec>()
            {
                Id = rec,
                Background = static (borderBox, isHovered, index) =>
                {
                    if (index % 2 == 0)
                    {
                        Widgets.DrawLightHighlight(borderBox);
                    }

                    if (isHovered)
                    {
                        Widgets.DrawHighlight(borderBox);
                    }
                },
            };

            for (int j = 0; j < columns.Count; j++)
            {
                var column = columns[j];
                var cellProps = headerRow.Cells[j].Props;
                List<Widget> cellContent;

                try
                {
                    cellContent = [column.Worker.GetTableCellContent(rec)];
                }
                catch
                {
                    cellContent = [];
                }

                var cell = new Widget_TableCell(cellContent)
                {
                    Padding = cellPadding,
                    Props = cellProps,
                };

                row.AddCell(cell);
            }

            AddBodyRow(row);
        }
    }
    private void SortRowsByColumn(ColumnDef column)
    {
        if (SortColumn == column)
        {
            SortDirection = (SortDirection)((int)SortDirection * -1);
        }
        else
        {
            SortColumn = column;
        }

        BodyRows.Sort((r1, r2) =>
            SortColumn.Worker.Compare(
                ((Widget_TableRow<ThingRec>)r1).Id,
                ((Widget_TableRow<ThingRec>)r2).Id
            ) * (int)SortDirection
        );
    }
}
