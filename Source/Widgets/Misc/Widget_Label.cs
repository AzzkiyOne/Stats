using UnityEngine;
using Verse;

namespace Stats;

// Maybe text shouldn't be a widget, but rather just a text node with "Size" prop?
// You can't set a width of a text. It must always have the width based on the text.
public sealed class Widget_Label
    : Widget_Drawable
{
    private readonly string Text;
    protected override Vector2 ContentSize { get; }
    public Widget_Label(string text, WidgetStyle? style = null)
        : base(style)
    {
        Text = text;
        ContentSize = Verse.Text.CalcSize(Text);
    }
    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        Verse.Text.Anchor = Style.TextAlign;

        Widgets.Label(rect, Text);

        Verse.Text.Anchor = Constants.DefaultTextAnchor;
    }
}
