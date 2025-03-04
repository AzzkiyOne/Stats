using System;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableCell_Body : IWidget_TableCell
{
    public float MinWidth => Widget_Table.CellPadding * 2f + Content?.MinWidth ?? 0f;
    public float Width
    {
        get => MasterCell.Width;
        set => MasterCell.Width = value;
    }
    public bool IsPinned => MasterCell.IsPinned;
    private readonly IWidget_TableCell MasterCell;
    private readonly IWidget? Content;
    public Widget_TableCell_Body(IWidget? content, IWidget_TableCell masterCell)
    {
        Content = content;
        MasterCell = masterCell;

        Width = Math.Max(MinWidth, Width);
    }
    public void Draw(Rect targetRect)
    {
        Content?.Draw(targetRect.ContractedBy(Widget_Table.CellPadding, 0f));
    }
}
