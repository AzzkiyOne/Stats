using Verse;

namespace Stats;

public class ColumnWorker_ContentSource : ColumnWorker<ICellWidget<string>>
{
    protected override ICellWidget<string>? CreateCell(ThingDef thingDef, ThingDef? stuffDef)
    {
        if (thingDef.modContentPack != null)
        {
            return new CellWidget_Str(
                thingDef.modContentPack.Name,
                thingDef.modContentPack.PackageIdPlayerFacing
            );
        }

        return null;
    }
}
