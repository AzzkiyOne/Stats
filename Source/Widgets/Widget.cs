using System.Diagnostics;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class Widget
    : IWidget
{
    public IWidget? Parent { private get; set; }
    public bool WidthIsUndef { protected get; set; } = true;
    public bool HeightIsUndef { protected get; set; } = true;
    private Vector2 Size = Vector2.zero;
    public Vector2 GetSize(in Vector2 containerSize)
    {
        return Size;
    }
    protected abstract Vector2 GetSize();
    public void Draw(Rect rect, in Vector2 containerSize)
    {
        DrawDebugInfo(rect);
        DrawContent(rect);
    }
    protected abstract void DrawContent(Rect rect);
    // One thing to remember, is that derived classes call this method in their
    // constructors to set their initial size.
    public void UpdateSize()
    {
        Size = GetSize();

        Parent?.UpdateSize();
    }
    [Conditional("DEBUG")]
    private void DrawDebugInfo(Rect rect)
    {
        if (Event.current.control && Mouse.IsOver(rect))
        {
            Widgets.DrawRectFast(rect, Color.cyan.ToTransparent(0.3f));
            //WriteDebugLine(this, $"{rect.width} x {rect.height}");
            //DebugInfoLines.Reverse();
            //TooltipHandler.TipRegion(rect, string.Join("\n", DebugInfoLines));
            TooltipHandler.TipRegion(rect, $"{GetType().Name}: {rect.width} x {rect.height} ({rect.x}, {rect.y})");
        }

        //DebugInfoLines.Clear();
    }
    //[Conditional("DEBUG")]
    //public static void WriteDebugLine(IWidget widget, string info)
    //{
    //    DebugInfoLines.Add($"{widget.GetType().Name}: {info}");
    //}
    //private static readonly List<string> DebugInfoLines = new(20);
}
