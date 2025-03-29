using UnityEngine;
using Verse;

namespace Stats;

// Maybe text shouldn't be a widget, but rather just a text node with "Size" prop?
// You can't set a width of a text. It must always have the width based on the text.
public sealed class Widget_Label
    : Widget
{
    private readonly string Text;
    public override Vector2 ContentSize { get; }
    public Widget_Label(string text, WidgetStyle? style = null)
        : base(style)
    {
        Text = text;
        ContentSize = Verse.Text.CalcSize(Text);
    }
    public override void DrawContentBox(Rect contentBox)
    {
        Verse.Text.Anchor = Style.TextAlign;

        Widgets.Label(contentBox, Text);

        Verse.Text.Anchor = Constants.DefaultTextAnchor;
    }
}
