using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private readonly RowCollection<Row> HeaderRows;
    private readonly RowCollection<ObjectRow> PinnedRows;
    private readonly RowCollection<ObjectRow> UnfilteredBodyRows;
    private readonly RowCollection<ObjectRow> FilteredBodyRows;
    private void PinRow(ObjectRow row)
    {
        PinnedRows.Add(row);
        SortRows(PinnedRows);
        FilteredBodyRows.Remove(row);
        UnfilteredBodyRows.Remove(row);
    }
    private void UnpinRow(ObjectRow row)
    {
        PinnedRows.Remove(row);

        if (ActiveFilters.Count == 0 || ObjectMatchesFilters(row.Object, ActiveFilters))
        {
            FilteredBodyRows.Add(row);
            SortRows(FilteredBodyRows);
        }

        UnfilteredBodyRows.Add(row);
        SortRows(UnfilteredBodyRows);
    }

    private sealed class RowCollection<TRow> : ICollection<TRow>, IReadOnlyCollection<TRow> where TRow : Row
    {
        private readonly List<TRow> Rows;
        public int Count => Rows.Count;
        public bool IsReadOnly => ((ICollection<TRow>)Rows).IsReadOnly;
        public float TotalHeight { get; private set; }
        public RowCollection(int capacity = 0)
        {
            Rows = new(capacity);
        }
        public RowCollection(RowCollection<TRow> rowCollection)
        {
            Rows = [.. rowCollection];
            TotalHeight = rowCollection.TotalHeight;
        }
        public void Add(TRow row)
        {
            Rows.Add(row);
            TotalHeight += row.Height;
        }
        public bool Remove(TRow row)
        {
            var rowWasRemoved = Rows.Remove(row);

            if (rowWasRemoved)
            {
                TotalHeight -= row.Height;
            }

            return rowWasRemoved;
        }
        public void Clear()
        {
            Rows.Clear();
            TotalHeight = 0f;
        }
        public bool Contains(TRow row)
        {
            return Rows.Contains(row);
        }
        public void CopyTo(TRow[] array, int arrayIndex)
        {
            Rows.CopyTo(array, arrayIndex);
        }
        public void ResetTo(RowCollection<TRow> rowCollection)
        {
            Rows.Clear();
            Rows.AddRange(rowCollection);
            TotalHeight = rowCollection.TotalHeight;
        }
        public void Sort(Comparison<TRow> comparison)
        {
            Rows.Sort(comparison);
        }
        public IEnumerator<TRow> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Rows.GetEnumerator();
        }
    }

    private class Row
    {
        private readonly Column[] Columns;
        private readonly Widget?[] Cells;
        public float Height;
        public Row(Column[] columns)
        {
            Columns = columns;
            Cells = new Widget?[columns.Length];
        }
        // Note to myself: Remember that table's row is not just simply a collection of cells.
        // Add(cell) method wouldn't even be correct for it. You can't just put a cell into a row.
        // Add(cell, columnIndex) is more correct version and is implemented below with an indexer.
        public Widget? this[int i]
        {
            set
            {
                Cells[i] = value;

                if (value != null)
                {
                    var cellSize = value.GetSize();
                    var column = Columns[i];

                    if (column.Width < cellSize.x)
                    {
                        column.Width = column.InitialWidth = cellSize.x;
                    }

                    if (Height < cellSize.y)
                    {
                        Height = cellSize.y;
                    }
                }
            }
        }
        public virtual bool Draw(
            Rect rect,
            float offsetX,
            bool drawPinnedColumns,
            float cellExtraWidth,
            int index
        )
        {
            var xMax = rect.width;
            rect.x = -offsetX;

            for (int i = 0; i < Cells.Length; i++)
            {
                if (rect.x >= xMax)
                {
                    break;
                }

                // It seems that this is faster than attaching column props object to a cell.
                var column = Columns[i];

                if (column.IsPinned != drawPinnedColumns)
                {
                    continue;
                }

                rect.width = column.Width + cellExtraWidth;
                if (rect.xMax > 0f)
                {
                    var cell = Cells[i];

                    if (cell != null)
                    {
                        var origTextAnchor = Text.Anchor;
                        Text.Anchor = column.TextAnchor;

                        try
                        {
                            // Basically, relative size extensions are not allowed on table cell widgets.
                            // Saves us some CPU cycles and is pointless to do anyway.
                            var cellSize = cell.GetSize();
                            rect.height = cellSize.y;

                            cell.Draw(rect, cellSize);
                        }
                        catch
                        {
                            // TODO: ?
                        }

                        Text.Anchor = origTextAnchor;
                    }
                }

                rect.x = rect.xMax;
            }

            return false;
        }
    }

    private sealed class ColumnLabelsRow : Row
    {
        public ColumnLabelsRow(Column[] columns) : base(columns)
        {
        }
        public override bool Draw(
            Rect rect,
            float offsetX,
            bool drawPinnedColumns,
            float cellExtraWidth,
            int index
        )
        {
            Verse.Widgets.DrawHighlight(rect);

            return base.Draw(rect, offsetX, drawPinnedColumns, cellExtraWidth, index);
        }
    }

    private sealed class ObjectRow : Row
    {
        public readonly TObject Object;
        private bool IsHovered = false;
        public ObjectRow(Column[] columns, TObject @object) : base(columns)
        {
            Object = @object;
        }
        public override bool Draw(
            Rect rect,
            float offsetX,
            bool drawPinnedColumns,
            float cellExtraWidth,
            int index
        )
        {
            var mouseIsOverRect = Mouse.IsOver(rect);

            if (Event.current.type == EventType.Repaint)
            {
                if (mouseIsOverRect)
                {
                    IsHovered = true;
                }

                // IsHovered may be true even if mouse is not over rect.
                if (IsHovered)
                {
                    Verse.Widgets.DrawHighlight(rect);
                }
                else if (index % 2 == 0)
                {
                    Verse.Widgets.DrawLightHighlight(rect);
                }

                if (mouseIsOverRect == false)
                {
                    IsHovered = false;
                }
            }

            base.Draw(rect, offsetX, drawPinnedColumns, cellExtraWidth, index);

            // This must go after cells to not interfere with their GUI events.
            if
            (
                Event.current.control
                && Event.current.type == EventType.MouseDown
                && mouseIsOverRect
            )
            {
                // Prevents flickering on pinning.
                IsHovered = false;

                return true;
            }

            return false;
        }
    }
}
