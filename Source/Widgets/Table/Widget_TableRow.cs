using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableRow
{
    public Widget_Table? Parent { private get; set; }
    public List<WidgetComp_TableCell> Cells { get; } = [];
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
    public Widget_TableRow(OnDraw onDraw)
    {
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
            Widgets.DrawHighlightSelected(rect);
        }

        var mouseIsOverRect = Mouse.IsOver(rect);

        if (mouseIsOverRect)
        {
            IsHovered = true;
        }

        DrawBG(rect, IsHovered, index);

        if (mouseIsOverRect == false)
        {
            IsHovered = false;
        }

        var xMax = rect.width;
        rect.x = -offsetX;

        foreach (var cell in Cells)
        {
            if (rect.x >= xMax)
            {
                break;
            }

            if (cell.Column.IsPinned != drawPinned)
            {
                continue;
            }

            rect.width = cell.Column.Width + cellExtraWidth;
            if (rect.xMax > 0f)
            {
                cell.DrawIn(rect);
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

        foreach (var cell in Cells)
        {
            var cellSize = cell.GetSize();

            cell.Column.Width = Mathf.Max(cell.Column.Width, cellSize.x);
            // This seems pointless (at least for now). But i'll just leave it for
            // correctness sake.
            Height = Mathf.Max(Height, cellSize.y);
        }

        return Height;
    }

    public delegate void OnDraw(Rect rect, bool isHovered, int index);
}

internal sealed class Widget_TableRow<IdType>
        : Widget_TableRow
{
    public IdType Id { get; }
    public Widget_TableRow(OnDraw onDraw, IdType id)
        : base(onDraw)
    {
        Id = id;
    }
}
