using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;

namespace Stats.Columns.Cells;

public interface IBooleanCell : ICell
{
    public bool Value { get; }
}

public readonly struct BooleanCell : IBooleanCell
{
    private static readonly Texture2D _textureTrue = Verse.Widgets.CheckboxOnTex;

    public float Width => 0f;
    public bool IsRefreshable => false;
    public bool Value { get; }

    public BooleanCell(bool value)
    {
        Value = value;
    }

    public void Draw(Rect rect) => Draw(rect, Value);

    public static void Draw(Rect rect, bool value)
    {
        if (value)
        {
            rect
                .ContractedByObjectTableCellPadding()
                .DrawTextureFitted(_textureTrue);
        }
    }
}
