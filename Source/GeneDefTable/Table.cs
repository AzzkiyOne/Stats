using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.GeneDefTable;

internal class Table : GenTable.Table<ColumnDef, GeneDef>
{
    public Table(
        List<ColumnDef> columns,
        List<GeneDef> genes
    ) : base(columns, genes.Select(gene => new GenTable.Row<ColumnDef, GeneDef>(gene, columns.Count)).ToList())
    {
    }
}
