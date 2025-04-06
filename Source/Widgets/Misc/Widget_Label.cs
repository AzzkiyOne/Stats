using UnityEngine;
using Verse;

namespace Stats;

// Maybe text shouldn't be a widget, but rather just a text node with "Size" prop?
// You can't set a width of a text. It must always have the width based on the text.
public sealed class Widget_Label
    : Widget
{
    private string _Text;
    public string Text
    {
        get => _Text;
        set
        {
            _Text = value;
            UpdateSize();
        }
    }
    public Widget_Label(string text)
    {
        Text = text;
    }
    protected override Vector2 GetSize()
    {
        return Verse.Text.CalcSize(Text);
    }
    protected override void DrawContent(Rect rect)
    {
        Widgets.Label(rect, Text);
    }
}
