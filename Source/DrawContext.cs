using System;
using UnityEngine;
using Verse;

namespace Stats;

internal readonly record struct GameFontCtx : IDisposable
{
    private readonly GameFont prevFont;
    public GameFontCtx(GameFont font)
    {
        prevFont = Text.Font;
        Text.Font = font;
    }
    public void Dispose()
    {
        Text.Font = prevFont;
    }
}

internal readonly record struct TextAnchorCtx : IDisposable
{
    private readonly TextAnchor prevAnchor;
    public TextAnchorCtx(TextAnchor anchor)
    {
        prevAnchor = Text.Anchor;
        Text.Anchor = anchor;
    }
    public void Dispose()
    {
        Text.Anchor = prevAnchor;
    }
}

internal readonly record struct ColorCtx : IDisposable
{
    private readonly Color prevColor;
    public ColorCtx(Color color)
    {
        prevColor = GUI.color;
        GUI.color = color;
    }
    public void Dispose()
    {
        GUI.color = prevColor;
    }
}

internal readonly record struct TextWordWrapCtx : IDisposable
{
    private readonly bool prevWordWrap;
    public TextWordWrapCtx(bool wordWrap)
    {
        prevWordWrap = Text.WordWrap;
        Text.WordWrap = wordWrap;
    }
    public void Dispose()
    {
        Text.WordWrap = prevWordWrap;
    }
}
