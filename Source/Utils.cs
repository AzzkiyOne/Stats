using System;
using System.Reflection;
using Verse;
using UE = UnityEngine;

namespace Stats.Utils;

static class GUI
{
    static public void DrawLineVertical(float x, float y, float length, UE::Color color)
    {
        using (new ColorContext(color))
        {
            Widgets.DrawLineVertical(x, y, length);
        }
    }
    public readonly record struct GameFontContext : IDisposable
    {
        private readonly GameFont prevFont;
        public GameFontContext(GameFont font)
        {
            prevFont = Text.Font;
            Text.Font = font;
        }
        public void Dispose()
        {
            Text.Font = prevFont;
        }
    }
    public readonly record struct TextAnchorContext : IDisposable
    {
        private readonly UE::TextAnchor prevAnchor;
        public TextAnchorContext(UE::TextAnchor anchor)
        {
            prevAnchor = Text.Anchor;
            Text.Anchor = anchor;
        }
        public void Dispose()
        {
            Text.Anchor = prevAnchor;
        }
    }
    public readonly record struct ColorContext : IDisposable
    {
        private readonly UE::Color prevColor;
        public ColorContext(UE::Color color)
        {
            prevColor = UE::GUI.color;
            UE::GUI.color = color;
        }
        public void Dispose()
        {
            UE::GUI.color = prevColor;
        }
    }
    public readonly record struct TextWordWrapContext : IDisposable
    {
        private readonly bool prevWordWrap;
        public TextWordWrapContext(bool wordWrap)
        {
            prevWordWrap = Text.WordWrap;
            Text.WordWrap = wordWrap;
        }
        public void Dispose()
        {
            Text.WordWrap = prevWordWrap;
        }
    }
}

static class Debug
{
    static public void TryDrawUIDebugInfo(UE::Rect targetRect, string text)
    {
        if (!InDebugMode)
        {
            return;
        }

        using (new GUI.GameFontContext(GameFont.Small))
        using (new GUI.TextAnchorContext(UE::TextAnchor.MiddleCenter))
        {
            var textSize = Text.CalcSize(text);
            const float padding = 5f;
            var rectWidth = textSize.x + padding + 10f;
            var rectHeight = textSize.y + padding;
            var rect = new UE::Rect(
                (targetRect.width - rectWidth) / 2,
                (targetRect.height - rectHeight) / 2,
                rectWidth,
                rectHeight
            );

            Widgets.DrawWindowBackground(rect);
            Widgets.Label(rect.ContractedBy(padding), text);
        }
    }
    static public bool InDebugMode => UE::Event.current.alt;
}

public interface IDrawable
{
    public void Draw(UE::Rect targetRect);
}

public static class Reflection
{
    public static readonly FieldInfo dialogInfoCardStuffField = typeof(Dialog_InfoCard)
        .GetField("stuff", BindingFlags.Instance | BindingFlags.NonPublic);
}