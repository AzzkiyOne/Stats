namespace Stats.Tables;

//public sealed class ApparelTableWorker : TableWorker<ApparelThing>, TableWorker<ApparelThing>.IStreaming
//{
//    public event Action<ApparelThing>? OnObjectAdded;
//    public event Action<ApparelThing>? OnObjectRemoved;
//    public sealed override IEnumerable<ApparelThing> InitialObjects
//    {
//        get
//        {
//            foreach (var thing in Find.Maps.GetSpawnedThings())
//            {
//                if (thing.def.apparel != null)
//                {
//                    yield return new ApparelThing(thing, thing.def.apparel);
//                }
//            }
//        }
//    }
//    public ApparelTableWorker(TableDef tableDef) : base(tableDef)
//    {
//        Globals.Events.ThingSpawned += thing =>
//        {
//            if (thing.def.apparel != null)
//            {
//                OnObjectAdded?.Invoke(new ApparelThing(thing, thing.def.apparel));
//            }
//        };
//        Globals.Events.ThingDespawned += thing =>
//        {
//            if (thing.def.apparel != null)
//            {
//                OnObjectRemoved?.Invoke(new ApparelThing(thing, thing.def.apparel));
//            }
//        };
//    }
//}
