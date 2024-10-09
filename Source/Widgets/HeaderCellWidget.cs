using UnityEngine;
using Verse;

namespace Stats;

internal sealed class HeaderCellWidget
{
    public ColumnDef Column { get; }
    public float MinWidth { get; set; } = TableWidget.CellMinWidth;
    public HeaderCellWidget(ColumnDef column)
    {
        Column = column;

        if (column.Icon != null)
        {
            MinWidth += TableWidget.RowHeight;
        }
        else
        {
            MinWidth += Text.CalcSize(column.Label).x;
        }
    }
    public bool Draw(Rect targetRect, ColumnDef? sortColumn, SortDirection sortDirection)
    {
        if (sortColumn == Column)
        {
            Widgets.DrawBoxSolid(
                sortDirection == SortDirection.Ascending
                    ? targetRect.BottomPartPixels(4f)
                    : targetRect.TopPartPixels(4f),
                Color.yellow
            );
        }

        var contentRect = targetRect.ContractedBy(TableWidget.CellPadding, 0f);

        if (Column.Icon != null)
        {
            contentRect = Column.CellTextAnchor switch
            {
                TextAnchor.LowerRight => contentRect.RightPartPixels(TableWidget.RowHeight),
                TextAnchor.LowerCenter => contentRect,
                _ => contentRect.LeftPartPixels(TableWidget.RowHeight)
            };
            Widgets.DrawTextureFitted(contentRect, Column.Icon, 1f);
        }
        else
        {
            Text.Anchor = Column.CellTextAnchor;
            Widgets.Label(contentRect, Column.Label);
            Text.Anchor = Constants.DefaultTextAnchor;
        }

        TooltipHandler.TipRegion(targetRect, new TipSignal(Column.Description));
        Widgets.DrawHighlightIfMouseover(targetRect);

        return Widgets.ButtonInvisible(targetRect);
    }
}
