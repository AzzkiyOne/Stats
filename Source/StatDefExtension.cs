using Verse;

namespace Stats;

public class StatDefExtension : DefModExtension
{
    public int diffMult = 1;
    [MustTranslate]
    public string label = "";
    public float minWidth = 75f;
    public GenTable_ColumnType type = GenTable_ColumnType.Number;
}
