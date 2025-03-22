using System.Collections.Generic;
using UnityEngine;

namespace Stats;

internal class Widget_Container_Ver
    : Widget
{
    public float Gap { get; set; } = 0f;
    public Widget_Container_Ver(List<Widget> children) : base(children)
    {
    }
    protected override IEnumerable<Rect> GetLayout(Vector2? contentBoxSize)
    {
        var y = 0f;

        foreach (var child in Children)
        {
            var childPos = new Vector2(0f, y);
            var childMarginBoxSize = child.GetMarginBoxSize(contentBoxSize);

            yield return new Rect(childPos, childMarginBoxSize);

            y += childMarginBoxSize.y + Gap;
        }
    }
}
