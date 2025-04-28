using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal class TableRow
{
    public Table? Parent { private get; set; }
    private readonly List<Widget> Cells;
    public float Height = 0f;
    private bool _IsHidden = false;
    public bool IsHidden
    {
        set
        {
            _IsHidden = value;
            Parent?.ScheduleLayoutRecalc();
        }
    }
    private readonly OnDraw DrawBG;
    private bool IsHovered = false;
    private bool _IsSelected = false;
    private bool IsSelected
    {
        set
        {
            _IsSelected = value;
            Parent?.ScheduleLayoutRecalc();
        }
    }
    public bool IsVisible => !_IsHidden || _IsSelected;
    public TableRow(List<Widget> cells, OnDraw onDraw)
    {
        Cells = cells;
        DrawBG = onDraw;
    }
    public void Draw(
        Rect rect,
        float offsetX,
        bool drawPinned,
        float cellExtraWidth,
        int index
    )
    {
        if (_IsSelected)
        {
            Verse.Widgets.DrawHighlightSelected(rect);
        }

        var mouseIsOverRect = Mouse.IsOver(rect);

        if (mouseIsOverRect)
        {
            IsHovered = true;
        }

        if (Event.current.type == EventType.Repaint)
        {
            DrawBG(rect, IsHovered, index);
        }

        if (mouseIsOverRect == false)
        {
            IsHovered = false;
        }

        var xMax = rect.width;
        rect.x = -offsetX;

        for (int i = 0; i < Cells.Count; i++)
        {
            if (rect.x >= xMax)
            {
                break;
            }

            // It seems that this is faster than attaching column props object to a cell.
            var column = Parent!.Columns[i];

            if (column.IsPinned != drawPinned)
            {
                continue;
            }

            rect.width = column.Width + cellExtraWidth;
            if (rect.xMax > 0f)
            {
                try
                {
                    var origTextAnchor = Text.Anchor;
                    Text.Anchor = column.TextAnchor;

                    // Basically, relative size extensions are not allowed on table cell widgets.
                    // Saves us some CPU cycles and is pointless to do anyway.
                    var cellSize = Cells[i].GetSize();
                    rect.height = cellSize.y;

                    Cells[i].Draw(rect, cellSize);

                    Text.Anchor = origTextAnchor;
                }
                catch
                {
                    // TODO: ?
                }
            }

            rect.x = rect.xMax;
        }

        // This must go after cells to not interfere with their GUI events.
        if (Event.current.type == EventType.MouseDown && mouseIsOverRect)
        {
            IsSelected = !_IsSelected;
        }
    }
    public float RecalcLayout()
    {
        Height = 0f;

        for (int i = 0; i < Cells.Count; i++)
        {
            Widget? cell = Cells[i];
            var cellSize = cell.GetSize();
            var column = Parent!.Columns[i];

            column.Width = Mathf.Max(column.Width, cellSize.x);
            // This seems pointless (at least for now). But i'll just leave it for
            // correctness sake.
            Height = Mathf.Max(Height, cellSize.y);
        }

        return Height;
    }

    public delegate void OnDraw(Rect rect, bool isHovered, int index);
}

internal sealed class TableRow<TId>
        : TableRow
{
    public TId Id { get; }
    public TableRow(List<Widget> cells, OnDraw onDraw, TId id)
        : base(cells, onDraw)
    {
        Id = id;
    }
}
