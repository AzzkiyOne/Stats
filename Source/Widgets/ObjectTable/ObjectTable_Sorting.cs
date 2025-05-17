using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private ColumnWorker<TObject> SortColumn;
    private int SortDirection = SortDirectionDescending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private static readonly Color SortIndicatorColor = Color.yellow.ToTransparent(0.3f);
    private const float SortIndicatorHeight = 5f;
    private void SortRowsByColumn(ColumnWorker<TObject> columnWorker)
    {
        if (SortColumn == columnWorker)
        {
            SortDirection *= -1;
        }
        else
        {
            SortColumn = columnWorker;
        }

        // TODO: Handle exception.
        BodyRows.Sort((r1, r2) => SortColumn.Compare(r1.Object, r2.Object) * SortDirection);
    }
}
