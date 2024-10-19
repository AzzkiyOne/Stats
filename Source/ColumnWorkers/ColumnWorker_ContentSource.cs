namespace Stats;

public class ColumnWorker_ContentSource : ColumnWorker<ICellWidget<string>>
{
    protected override ICellWidget<string>? CreateCell(ThingRec thing)
    {
        if (thing.Def.modContentPack != null)
        {
            return new CellWidget_Str(
                thing.Def.modContentPack.Name,
                thing.Def.modContentPack.PackageIdPlayerFacing
            );
        }

        return null;
    }
}
