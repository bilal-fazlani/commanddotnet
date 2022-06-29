# CommandDotNet.NewerReleasesAlerts

## 4.0.1

* update to dotnet 6

## 3.0.1

remove nuget package refs no longer required after move to net5.0

## 3.0.0

CommandDotNet.NewerReleasesAlerts targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.

## 2.0.1

Just some code cleanup. No new features.

## 2.0.0

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

### MiddlewareSteps

add MiddlewareSteps.NewerReleaseAlerts, declaring the stage and order within the stage the middleware is registered
