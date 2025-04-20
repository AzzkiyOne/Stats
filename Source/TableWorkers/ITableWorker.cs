using System.Collections.Generic;

namespace Stats.TableWorkers;

public interface ITableWorker
{
    TableDef TableDef { get; set; }
    IEnumerable<ThingAlike> GetRecords();
}
