using UnityEngine;
using Verse;

namespace Stats;

public static class GUIStyles
{
    private static readonly GUIStyle _baseStyle = new(Verse.Text.fontStyles[1])
    {
        wordWrap = false
    };

    internal static class Global
    {
        internal const float Pad = 10f;
        internal const float PadSm = 5f;
        internal const float PadXs = 3f;
        internal const float EstimatedInputFieldInnerPadding = 2f;
        internal const float ButtonSubtleContentHoverOffset = 2f;
        internal static readonly Color HighlightDraggedColor = Verse.Widgets.HighlightStrongBgColor.ToTransparent(0.5f);
    }

    internal static class MainTabWindow
    {
        internal const float ToolbarWidth = 40f;
        internal const float IconPadding = 5f;
        internal static readonly Color BorderColor = new(1f, 1f, 1f, 0.4f);
    }

    public static class Text
    {
        public const float LineHeight = Verse.Text.SmallFontHeight;
        public static readonly Color ColorHighlight = new(1f, 0.98f, 0.62f);
        public static readonly Color ColorSecondary = Color.grey;
    }

    internal static class Table
    {
        internal const float RowHeight = Text.LineHeight + TableCell.PadVer * 2f;
        internal const float HeadersRowHeight = RowHeight;
        internal static readonly Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
        internal static Color FixedPartSeparatorLineColor => HeadersRowBGColor;
        internal static readonly Color HeadersRowBGColor = GenColor.FromBytes(56, 56, 60);
    }

    internal static class TableToolbar
    {
        internal const float Height = Text.LineHeight + TableCell.PadVer * 2f;
        internal const float Gap = Global.PadSm;
    }

    internal static class TableToolbarButton
    {
        internal const float IconWidth = Text.LineHeight;
        internal const float PadHor = Global.Pad;
        internal const float PadVer = TableCell.PadVer;
        internal static readonly GUIStyle LabelStyle = new(_baseStyle)
        {
            alignment = TextAnchor.MiddleLeft,
        };
    }

    public static class TableCell
    {
        public static readonly GUIStyle String;
        public static readonly GUIStyle StringNoPad;
        public static readonly GUIStyle Number;
        public static readonly GUIStyle NumberNoPad;
        public static readonly GUIStyle Boolean;
        public static readonly GUIStyle BooleanNoPad;
        public const float ContentSpacing = PadHor / 2f;
        public const float PadHor = _PadHor;
        public const float PadVer = _PadVer;

        private const int _PadHor = 16;
        private const int _PadVer = 4;

        static TableCell()
        {
            RectOffset padding = new(_PadHor, _PadHor, _PadVer, _PadVer);

            StringNoPad = new GUIStyle(_baseStyle);
            StringNoPad.alignment = TextAnchor.LowerLeft;
            String = new GUIStyle(StringNoPad);
            String.padding = padding;

            NumberNoPad = new GUIStyle(_baseStyle);
            NumberNoPad.alignment = TextAnchor.LowerRight;
            Number = new GUIStyle(NumberNoPad);
            Number.padding = padding;

            BooleanNoPad = new GUIStyle(_baseStyle);
            BooleanNoPad.alignment = TextAnchor.LowerCenter;
            Boolean = new GUIStyle(BooleanNoPad);
            Boolean.padding = padding;
        }
    }
}
