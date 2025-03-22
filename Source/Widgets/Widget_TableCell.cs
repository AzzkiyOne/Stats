using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

internal class Widget_TableCell
    : Widget
{
    public required Properties Props { get; set; }
    public Widget_TableCell(List<Widget> children)
        : base(children)
    {
    }
    protected override IEnumerable<Rect> GetLayout(Vector2? contentBoxSize)
    {
        var content = Children[0];

        if (content != null)
        {
            yield return new Rect(
                Vector2.zero,
                content.GetMarginBoxSize(contentBoxSize)
            );
        }
    }
    protected override void DrawContentBox(Rect contentBox)
    {
        Text.Anchor = Props.TextAnchor;

        base.DrawContentBox(contentBox);

        Text.Anchor = Constants.DefaultTextAnchor;
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
        public required TextAnchor TextAnchor { get; set; }
    }
}
