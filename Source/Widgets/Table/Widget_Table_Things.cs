using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class Widget_Table_Things
    : Widget_Table
{
    private ColumnDef SortColumn = ColumnDefOf.Name;
    private SortDirection SortDirection = SortDirection.Descending;
    private const float cellPaddingHor = 15f;
    private const float cellPaddingVer = 5f;
    public Widget_Table_Things(TableDef tableDef)
        : base()
    {
        List<ColumnDef> columns = [ColumnDefOf.Name, .. tableDef.columns];

        // Headers

        var headerRow = new Widget_TableRow(DrawHeaderRowBG);

        foreach (var column in columns)
        {
            headerRow.AddCell(CreateHeaderCell(column));
        }

        AddHeaderRow(headerRow);

        // Body

        foreach (var rec in tableDef.Worker.GetRecords())
        {
            var row = new Widget_TableRow<ThingRec>(DrawBodyRowBG, rec);

            for (int j = 0; j < columns.Count; j++)
            {
                var column = columns[j];
                var columnProps = headerRow.Cells[j].Column;

                row.AddCell(CreateBodyCell(column, rec, columnProps));
            }

            AddBodyRow(row);
        }

        SortRowsByColumn(SortColumn);
    }
    private Widget_TableCell CreateHeaderCell(ColumnDef column)
    {
        var columnProps = new ColumnProps()
        {
            IsPinned = column == ColumnDefOf.Name,
        };
        Widget iconOrLabel;

        if (column.Icon != null)
        {
            var iconStyle = new WidgetStyle()
            {
                Width = Text.LineHeight,
                Height = Text.LineHeight,
            };
            iconOrLabel = new Widget_Texture(column.Icon, iconStyle);

            if (column.Worker.CellStyle == ColumnCellStyle.Number)
            {
                iconOrLabel =
                    new Widget_Container_Hor(
                        [new Widget_Addon_Margin_Rel(iconOrLabel, 100, 0f, 0f, 0f)]
                    );
            }
            else if (column.Worker.CellStyle == ColumnCellStyle.Boolean)
            {
                iconOrLabel =
                    new Widget_Container_Hor(
                        [new Widget_Addon_Margin_Rel(iconOrLabel, 50, 0f)]
                    );
            }
        }
        else
        {
            var labelStyle = new WidgetStyle()
            {
                TextAlign = (TextAnchor)column.Worker.CellStyle,
            };
            iconOrLabel = new Widget_Label(column.LabelCap, labelStyle);
        }

        void onDrawCell(ref Rect rect)
        {
            Widgets.DrawHighlightIfMouseover(rect);

            if (SortColumn == column)
            {
                Widgets.DrawBoxSolid(
                    SortDirection == SortDirection.Ascending
                        ? rect.BottomPartPixels(5f)
                        : rect.TopPartPixels(5f),
                    Color.yellow.ToTransparent(0.3f)
                );
            }

            if (Widgets.ButtonInvisible(rect))
            {
                if (Event.current.control)
                {
                    columnProps.IsPinned = !columnProps.IsPinned;
                }
                else
                {
                    SortRowsByColumn(column);
                }
            }
        }

        return
            new Widget_TableCell_Normal(
                new Widget_Addon_Generic(
                    new Widget_Addon_Tooltip(
                        new Widget_Addon_Padding(
                            iconOrLabel,
                            cellPaddingHor,
                            cellPaddingVer
                        ),
                        column.description
                    ),
                    onDrawCell
                ),
                columnProps
            );
    }
    private Widget_TableCell CreateBodyCell(
        ColumnDef column,
        ThingRec rec,
        ColumnProps columnProps
    )
    {
        Widget? cellContent = null;

        try
        {
            cellContent = column.Worker.GetTableCellContent(rec);
        }
        catch
        {
        }

        if (cellContent == null)
        {
            return new Widget_TableCell_Empty(columnProps);
        }
        else
        {
            return
                new Widget_TableCell_Normal(
                    new Widget_Addon_Padding(
                        cellContent,
                        cellPaddingHor,
                        cellPaddingVer
                    ),
                    columnProps
                );
        }
    }
    private static void DrawHeaderRowBG(Rect borderBox, bool _, int __)
    {
        Widgets.DrawHighlight(borderBox);
        Widgets.DrawLineHorizontal(
            borderBox.x,
            borderBox.yMax - 1f,
            borderBox.width,
            StatsMainTabWindow.BorderLineColor
        );
    }
    private static void DrawBodyRowBG(Rect borderBox, bool isHovered, int index)
    {
        if (isHovered)
        {
            Widgets.DrawHighlight(borderBox);
        }
        else if (index % 2 == 0)
        {
            Widgets.DrawLightHighlight(borderBox);
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
