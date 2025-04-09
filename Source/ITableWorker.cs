using System.Collections.Generic;

namespace Stats;

public interface ITableWorker
{
    TableDef TableDef { get; set; }
    IEnumerable<ThingRec> GetRecords();
}
