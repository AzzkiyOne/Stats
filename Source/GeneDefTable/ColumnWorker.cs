using Verse;

namespace Stats.GeneDefTable;

public abstract class ColumnWorker : GenTable.ColumnWorker<GeneDef>
{
    public ColumnDef Column { get; set; }
}

public class ColumnWorker_Label : ColumnWorker
{
    public override GenTable.Cell? GetCell(GeneDef gene)
    {
        return new GenTable.Cell_DefRef(Column, gene);
    }
}
