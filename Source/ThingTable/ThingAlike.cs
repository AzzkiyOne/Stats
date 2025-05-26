using Verse;

namespace Stats.ThingTable;

/*

Notes:

---

Using structs results in less (long-lived) objects allocated on heap. Which means less work for GC.

---

For (global) caches to work we need value equality:

var thingDef = DefDatabase<ThingDef>.GetNamed("...");
var stuffDef = DefDatabase<ThingDef>.GetNamed("...");

var thing1 = new ThingAlike(thingDef, stuffDef);
var thing2 = new ThingAlike(thingDef, stuffDef);

thing1 == thing2;// true
  
We don't have to necessarily use a struct for this, but given how and what ThingAlike is used for, it makes sense for it to be a struct. Basically it is a lighter version of RimWorld.StatRequest.

*/

public readonly record struct ThingAlike(ThingDef Def, ThingDef? StuffDef = null);
