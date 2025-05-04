using System.Collections.Generic;

namespace Stats.TableWorkers;

public abstract class TableWorker<T>
{
    public abstract IEnumerable<T> GetRecords();
}
