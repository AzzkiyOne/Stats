using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ThingTable;

[StaticConstructorOnStartup]
public static class ColumnLabelFormat
{
    private static readonly Texture2D ArmorIcon;
    private static readonly Texture2D DamageToIcon;
    static ColumnLabelFormat()
    {
        ArmorIcon = ContentFinder<Texture2D>.Get("Things/Pawn/Humanlike/Apparel/FlakVest/FlakVest");
        DamageToIcon = ContentFinder<Texture2D>.Get("UI/Commands/FireAtWill");
    }
    public static Widget ArmorRating(ColumnDef columnDef, ColumnCellStyle _)
    {
        return new HorizontalContainer(
            [
                new InlineTexture(ArmorIcon).Color(new(0.4f,0.4f,0.4f)).PaddingRel(1f, 0f, 0f, 0f),
                new InlineTexture(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor)
            ],
            Globals.GUI.PadXs,
            true
        );
    }
    public static Widget DamageFactorTo(ColumnDef columnDef, ColumnCellStyle _)
    {
        return new HorizontalContainer(
            [
                new InlineTexture(DamageToIcon, 1.2f).PaddingRel(1f, 0f, 0f, 0f),
                new Label("%"),
                new InlineTexture(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor)
            ],
            Globals.GUI.PadXs,
            true
        );
    }
}
