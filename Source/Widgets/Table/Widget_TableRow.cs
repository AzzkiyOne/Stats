using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableRow
{
    public List<WidgetComp_TableCell> Cells { get; } = [];
    public float Height = 0f;
    public bool IsHidden = false;
    private readonly OnDraw DrawBG;
    private bool IsHovered = false;
    public bool IsSelected = false;
    public Widget_TableRow(OnDraw onDraw)
    {
        DrawBG = onDraw;
    }
    public void Draw(
        Rect rect,
        float offsetX,
        bool drawPinned,
        float cellExtraWidth,
        int index,
        Widget_Table parent
    )
    {
        var mouseIsOverRect = Mouse.IsOver(rect);

        if (mouseIsOverRect) IsHovered = true;

        if (IsSelected) Widgets.DrawHighlightSelected(rect);
        DrawBG(ref rect, IsHovered, index);

        if (mouseIsOverRect == false) IsHovered = false;

        // Cells
        var cellRect = new Rect(-offsetX, rect.y, 0f, rect.height);

        foreach (var cell in Cells)
        {
            if (cell.Column.IsPinned != drawPinned) continue;
            if (cellRect.x >= rect.width) break;

            cellRect.width = cell.Column.Width + cellExtraWidth;

            if (cellRect.xMax > 0f)
            {
                cell.DrawIn(cellRect);
            }

            cellRect.x = cellRect.xMax;
        }

        // This must go after cells to not interfere with their GUI events.
        if (Event.current.type == EventType.MouseDown && mouseIsOverRect)
        {
            // It is important to switch row's selected flag before
            // layout recalculation, because it skips hidden and unselected rows.
            IsSelected = !IsSelected;
            if (IsSelected == false && IsHidden) parent.RecalcLayout();
            // Not sure if this is usefull.
            // Better would be to stop rendering altogether, because of layout
            // recalculation.
            Event.current.Use();
        }
    }

    internal delegate void OnDraw(ref Rect rect, bool isHovered, int index);
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
