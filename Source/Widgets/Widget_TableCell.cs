using System;
using UnityEngine;

namespace Stats;

internal class Widget_TableCell
    : Widget
{
    public Properties Props { get; }
    private readonly Widget? Widget;
    public bool IsEmpty => Widget == null;
    public override Vector2 ContentSize { get; }
    public Widget_TableCell(
        Widget? widget,
        Properties props,
        WidgetStyle? style = null
    )
        : base(style)
    {
        Props = props;
        Widget = widget;
        ContentSize = Widget?.GetMarginBoxSize() ?? Vector2.zero;
    }
    public override void DrawContentBox(Rect contentBox)
    {
        Widget?.DrawIn(contentBox);
    }
    public class Properties
    {
        public bool IsPinned { get; set; } = false;
        private float _Width = 0f;
        public float Width
        {
            get => _Width;
            set => _Width = Math.Max(_Width, value);
        }
    }
}
