using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.Widgets;
using UnityEngine;
using static Stats.GUIStyles.TableCell;

namespace Stats.Columns.Cells;

public interface IThingDefCell : ICell
{
    public Verse.ThingDef? Value { get; }
    public string? Text { get; }
}

public readonly struct ThingDefCell : IThingDefCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public Verse.ThingDef? Value { get; }
    public string? Text { get; }

    private readonly ThingDefIcon? _icon;
    private readonly float _iconWidth;

    public ThingDefCell(Verse.ThingDef value)
    {
        Value = value;
        Text = value.LabelCap;
        float textWidth = Verse.Text.CalcSize(Text).x;
        _icon = new ThingDefIcon(value);
        _iconWidth = _icon.Size.x;
        Width = _iconWidth + ContentSpacing + textWidth;
    }

    public void Draw(Rect rect)
    {
        if (Value != null)
        {
            rect
                .ContractedByObjectTableCellPadding()
                .CutLeft(out Rect iconRect, _iconWidth)
                .CutLeft(ContentSpacing)
                .TakeRest(out Rect labelRect);

            if (Event.current.type == EventType.Repaint)
            {
                _icon!.Draw(iconRect);
                Text!.Draw(labelRect, StringNoPad);
            }

            bool iconWasClicked = iconRect.ButtonGhostly();
            if (iconWasClicked)
            {
                Value.OpenInfoDialog();
            }
        }
    }
}
