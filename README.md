[Subscribe to the mod on Steam!](https://steamcommunity.com/sharedfiles/filedetails/?id=3479566439)

# Naming convention

The project uses standard C# naming convention for classes, namespaces, etc.

There are some exceptions to the rules, which are listed below.

Widget class names don't use `Widget` suffix, as opposite to, for example, table column workers. That is because, often times, a widget's name is enough to conclude that something is a widget. For example, one can tell with a high degree of certainty, that `Label` or `Button` is a widget. Now if we take for example `StatColumnWorker` and remove the `ColumnWorker` suffix, we'll be left with just `Stat`. `ColumnWorker` suffix provides important context, which makes the name unambiguous.

# Column worker notes

Why to use empty collections/strings in place of `null`?

This is just faster down the line. Saves a bunch of `null` checks. Filtering/sorting must be fast because it is done live.
