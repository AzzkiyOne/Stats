using Verse;

namespace Stats;

/*

Reasons for ThingRec to be defined as a struct instead of a class:

- If you want enum-filters to display only options that are valid for a table, you'll need to pass all of the records to filter-widget factory. Since factory only needs to derive some data from these records, the records are short-lived and so they better be alocated on a stack. For example, "Created at" column filter should probably only display crafting benches on which at least one item in the table can be created.
- Since a struct is a part of an object, rather than a separate object, this will produce less long-lived objects.

*/
public readonly record struct ThingRec(ThingDef Def, ThingDef? StuffDef = null);
//public record class ThingRec(ThingDef Def, ThingDef? StuffDef = null);
