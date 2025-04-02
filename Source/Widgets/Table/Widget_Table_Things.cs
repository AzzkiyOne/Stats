using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed class Widget_Table_Things
    : Widget_Table
{
    private ColumnDef SortColumn = ColumnDefOf.Name;
    private SortDirection SortDirection = SortDirection.Descending;
    private const float cellPadHor = 15f;
    private const float cellPadVer = 5f;
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
        IWidget cell;

        if (column.Icon != null)
        {
            cell = new Widget_Texture(column.Icon);
            cell = new WidgetComp_Size_Abs(cell, Text.LineHeight);

            if (column.Worker.CellStyle == ColumnCellStyle.Number)
            {
                cell = new WidgetComp_Size_Inc_Rel(cell, 1f, 0f, 0f, 0f);
            }
            else if (column.Worker.CellStyle == ColumnCellStyle.Boolean)
            {
                cell = new WidgetComp_Size_Inc_Rel(cell, 0.5f, 0f);
            }

            cell = new Widget_Container_Single(cell);
        }
        else
        {
            cell = new Widget_Label(column.LabelCap);
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

        cell = new WidgetComp_Size_Inc_Abs(cell, cellPadHor, cellPadVer);
        cell = new WidgetComp_Width_Rel(cell, 1f);
        cell = new WidgetComp_Tooltip(cell, column.description);
        cell = new WidgetComp_Generic(cell, onDrawCell);

        return new Widget_TableCell_Normal(cell, columnProps, column.Worker.CellStyle);
    }
    private Widget_TableCell CreateBodyCell(
        ColumnDef column,
        ThingRec rec,
        ColumnProps columnProps
    )
    {
        IWidget? cell = null;

        try
        {
            cell = column.Worker.GetTableCellContent(rec);
        }
        catch
        {
        }

        if (cell == null)
        {
            return new Widget_TableCell_Empty(columnProps);
        }
        else
        {
            cell = new WidgetComp_Size_Inc_Abs(cell, cellPadHor, cellPadVer);
            cell = new WidgetComp_Width_Rel(cell, 1f);

            return new Widget_TableCell_Normal(cell, columnProps, column.Worker.CellStyle);
        }
    }
    private static void DrawHeaderRowBG(ref Rect borderBox, in bool _, in int __)
    {
        Widgets.DrawHighlight(borderBox);
        Widgets.DrawLineHorizontal(
            borderBox.x,
            borderBox.yMax - 1f,
            borderBox.width,
            StatsMainTabWindow.BorderLineColor
        );
    }
    private static void DrawBodyRowBG(ref Rect borderBox, in bool isHovered, in int index)
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
