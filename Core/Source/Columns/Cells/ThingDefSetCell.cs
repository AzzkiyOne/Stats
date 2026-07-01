using System.Collections.Generic;
using Stats.Utils;
using UnityEngine;

namespace Stats.Columns.Cells;

public interface IThingDefSetCell : ICell
{
    public IReadOnlyCollection<Verse.ThingDef?>? Value { get; }
}

public readonly struct ThingDefSetCell : IThingDefSetCell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public IReadOnlyCollection<Verse.ThingDef?>? Value { get; }

    public ThingDefSetCell(IReadOnlyCollection<Verse.ThingDef?> value)
    {
        Value = value;
    }

    public void Draw(Rect rect)
    {
        if (Value?.Count > 0)
        {
            rect.Label("TODO", GUIStyles.TableCell.String);
        }
    }
}
