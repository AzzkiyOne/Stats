Why to use empty collections/strings in place of `null`?

This is just faster down the line. Saves a bunch of `null` checks. Filtering/sorting must be fast because it is done live.