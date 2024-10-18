using Verse;

namespace Stats;

public interface IColumnWorker
{
    ColumnDef ColumnDef { get; set; }
    ICellWidget? GetCell(ThingDef thingDef, ThingDef? stuffDef);
}

public interface IColumnWorker<CellType> : IColumnWorker where CellType : ICellWidget
{
    // TODO
}
