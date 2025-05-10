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
    public static Widget ArmorRating(IColumnDef columnDef, ColumnCellStyle _)
    {
        return new HorizontalContainer(
            [
                new Icon(ArmorIcon).Color(new(105,105,105)).PaddingRel(1f, 0f, 0f, 0f),
                new Icon(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor)
            ],
            Globals.GUI.PadSm,
            true
        );
    }
    public static Widget DamageFactorTo(IColumnDef columnDef, ColumnCellStyle _)
    {
        return new HorizontalContainer(
            [
                new Icon(DamageToIcon, 1.2f).PaddingRel(1f, 0f, 0f, 0f),
                new Label("%"),
                new Icon(columnDef.Icon!, columnDef.IconScale).Color(columnDef.IconColor)
            ],
            Globals.GUI.PadXs,
            true
        );
    }
}
