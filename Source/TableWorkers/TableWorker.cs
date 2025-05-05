using System.Collections.Generic;

namespace Stats;

public abstract class TableWorker<T>
{
    public abstract IEnumerable<T> GetRecords();
}
