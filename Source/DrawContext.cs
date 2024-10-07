using System;
using UnityEngine;
using Verse;

namespace Stats;

internal readonly record struct GameFontCtx : IDisposable
{
    private readonly GameFont OrigFont;
    public GameFontCtx(GameFont font)
    {
        OrigFont = Text.Font;
        Text.Font = font;
    }
    public void Dispose()
    {
        Text.Font = OrigFont;
    }
}

internal readonly record struct TextAnchorCtx : IDisposable
{
    private readonly TextAnchor OrigAnchor;
    public TextAnchorCtx(TextAnchor anchor)
    {
        OrigAnchor = Text.Anchor;
        Text.Anchor = anchor;
    }
    public void Dispose()
    {
        Text.Anchor = OrigAnchor;
    }
}

internal readonly record struct ColorCtx : IDisposable
{
    private readonly Color OrigColor;
    public ColorCtx(Color color)
    {
        OrigColor = GUI.color;
        GUI.color = color;
    }
    public void Dispose()
    {
        GUI.color = OrigColor;
    }
}

internal readonly record struct TextWordWrapCtx : IDisposable
{
    private readonly bool OrigWordWrap;
    public TextWordWrapCtx(bool wordWrap)
    {
        OrigWordWrap = Text.WordWrap;
        Text.WordWrap = wordWrap;
    }
    public void Dispose()
    {
        Text.WordWrap = OrigWordWrap;
    }
}
