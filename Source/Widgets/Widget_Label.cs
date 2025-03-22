using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats;

// Maybe text shouldn't be a widget, but rather just a text node with "Size" prop?
// You can't set a width of a text. It must always have the width based on the text.
public sealed class Widget_Label
    : Widget
{
    private readonly string Text;
    public Widget_Label(string text) : base([])
    {
        Text = text;
    }
    protected override IEnumerable<Rect> GetLayout(Vector2? _)
    {
        yield return new Rect(Vector2.zero, Verse.Text.CalcSize(Text));
    }
    protected override void DrawContentBox(Rect contentBox)
    {
        Widgets.Label(contentBox, Text);
    }
}
