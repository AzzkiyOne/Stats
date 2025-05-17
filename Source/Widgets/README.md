# The difference between `WidgetWrapper` and `WidgetExtension`

- `WidgetWrapper` is meant to be used for encapsulation (ex. `MainTabWindowTitleBar`).
- `WidgetExtension` is meant to be used for widget extensions.

Both are technically decorators and could be implemented as a single `WidgetDecorator` class, but i've separated them because semantically, they are different: classes derived from `WidgetWrapper` are widgets, but classes derived from `WidgetExtension` are not.

`WidgetWrapper` members are sealed on purpose, to emphasize that this is just a wrapper. On the other hand, `WidgetExtension` members are open, because you'll want to override them to create a widget extension.

# !!! Important notes !!!

Don't draw the same widget at different places (no multiple parents). This will constantly invalidate widget's size cache.

## Note

It should be theoretically possible to draw the same widget at several different places. Size cache invalidation could be solved by a simple dummy-extension that will act as just a size cache holder. in theory, any extension can fulfill this role. So, the underlying widget will be the same instance, but at each location a different extension instance will be drawn instead.

```
SizeCacheHolderExtensionInstance_A
                                  \
SizeCacheHolderExtensionInstance_B--WidgetInstance
                                  /
SizeCacheHolderExtensionInstance_C
```

The only issue left is `Parent` property. It's function could be implemented by using `OnChange` event.
