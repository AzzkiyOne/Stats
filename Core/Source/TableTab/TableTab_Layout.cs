using System.Collections.Generic;
using UnityEngine;
using static Stats.GUIStyles.Table;

namespace Stats;

public sealed partial class TableTab<TRecord>
{
    private void RecalcLayout()
    {
        List<ColumnWidget> columns = _columns;
        int columnsCount = _columns.Count;
        int leftColumnsCount = _leftColumnsCount;
        float leftColumnsWidth = 0f;
        float rightColumnsWidth = 0f;
        for (int i = 0; i < columnsCount; i++)
        {
            ColumnWidget column = columns[i];
            if (column.IsManuallyResized == false)
            {
                column.RecalcWidth(_rows);
            }

            if (i < leftColumnsCount)
            {
                leftColumnsWidth += column.Width;
            }
            else
            {
                rightColumnsWidth += column.Width;
            }
        }

        float contentWidth = leftColumnsWidth + rightColumnsWidth;
        float topRowsHeight = _topRowsCount * RowHeight;
        float bottomRowsHeight = BottomRowsCount * RowHeight;
        float contentHeight = HeadersRowHeight + topRowsHeight + bottomRowsHeight;

        _topRowsHeight = topRowsHeight;
        _bottomRowsHeight = bottomRowsHeight;
        _leftColumnsWidth = leftColumnsWidth;
        _contentSize = new Vector2(contentWidth, contentHeight);
    }
}
