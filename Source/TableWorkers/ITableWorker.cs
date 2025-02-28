using System.Collections.Generic;

namespace Stats;

internal interface ITableWorker
{
    TableDef TableDef { get; set; }
    IEnumerable<ThingRec> GetRecords();
}
