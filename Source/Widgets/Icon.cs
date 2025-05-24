using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class Icon : InlineTexture
{
    public Icon(Texture2D texture, float scale = 1) : base(texture, scale)
    {
    }
    public Icon(Texture2D texture, out Icon instance) : base(texture)
    {
        instance = this;
    }
    protected override Vector2 CalcSize()
    {
        return new Vector2(Text.LineHeight, Text.LineHeight);
    }
}
