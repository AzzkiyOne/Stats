﻿using System;
using UnityEngine;
using Verse;

namespace Stats;

static class GUIUtils
{
    static public void DrawLineVertical(float x, float y, float length, Color color)
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
        private readonly TextAnchor prevAnchor;
        public TextAnchorContext(TextAnchor anchor)
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
        private readonly Color prevColor;
        public ColorContext(Color color)
        {
            prevColor = GUI.color;
            GUI.color = color;
        }
        public void Dispose()
        {
            GUI.color = prevColor;
        }
    }
}

static class Debug
{
    static public void TryDrawUIDebugInfo(Rect targetRect, string text)
    {
        if (!InDebugMode())
        {
            return;
        }

        using (new GUIUtils.GameFontContext(GameFont.Small))
        using (new GUIUtils.TextAnchorContext(TextAnchor.MiddleCenter))
        {
            var textSize = Text.CalcSize(text);
            const float arbitraryTextSizeOffset = 10f;
            const float padding = 5f;
            var rectWidth = textSize.x + padding + arbitraryTextSizeOffset;
            var rectHeight = textSize.y + padding;
            var rect = new Rect((targetRect.width - rectWidth) / 2, (targetRect.height - rectHeight) / 2, rectWidth, rectHeight);

            Widgets.DrawWindowBackground(rect);
            Widgets.Label(rect.ContractedBy(padding), text);
        }
    }
    static public bool InDebugMode()
    {
        return Event.current.alt;
    }
}