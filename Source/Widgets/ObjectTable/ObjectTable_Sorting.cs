using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private Column SortColumn;
    private int SortDirection;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private static readonly Color SortIndicatorColor = Color.yellow.ToTransparent(0.3f);
    private const float SortIndicatorHeight = 5f;
    private void SortAllRowsByColumn(Column column)
    {
        if (SortColumn == column)
        {
            SortDirection *= -1;
        }
        else
        {
            SortColumn = column;
        }

        // We keep filtered and unfiltered rows synchronized so we don't have to do sorting after filtering.
        // TODO: Handle exception.
        SortRows(UnfilteredBodyRows);
        SortRows(FilteredBodyRows);
        SortRows(PinnedRows);
    }
    private void SortRows(RowCollection<ObjectRow> rows)
    {
        rows.Sort((r1, r2) => SortColumn.Worker.Compare(r1.Object, r2.Object) * SortDirection);
    }
}
