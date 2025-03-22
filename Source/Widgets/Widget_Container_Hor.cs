using System.Collections.Generic;
using UnityEngine;

namespace Stats;

internal class Widget_Container_Hor
    : Widget
{
    public float Gap { get; set; } = 0f;
    public Widget_Container_Hor(List<Widget> children) : base(children)
    {
    }
    protected override IEnumerable<Rect> GetLayout(Vector2? contentBoxSize)
    {
        var x = 0f;

        foreach (var child in Children)
        {
            var childPos = new Vector2(x, 0f);
            var childMarginBoxSize = child.GetMarginBoxSize(contentBoxSize);

            yield return new Rect(childPos, childMarginBoxSize);

            x += childMarginBoxSize.x + Gap;
        }
    }
}
