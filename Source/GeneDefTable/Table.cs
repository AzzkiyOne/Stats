using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.GeneDefTable;

internal class Table : GenTable.Table<GeneDef>
{
    public Table(
        List<GenTable.IColumn<GeneDef>> columns,
        List<GeneDef> genes
    ) : base(columns, genes.Select(gene => new GenTable.Row<GeneDef>(gene, columns.Count)).ToList())
    {
    }
}
