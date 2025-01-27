# CommandDotNet.IoC.AutoFac

## 7.1.0

* Accidental increment. No features or fixes. Apologies for the noise.

## 7.0.0

* add support for net9
* drop support for net6 and net7

## 6.0.0

* upgrade to Autofac 8.0.0

## 5.0.2

* support dotnet 7

## 5.0.1

* update to dotnet 6
* use `NotNullWhen(true)` on TryResolve to inform compiler the value is null when the method returns false.

## 4.0.1

remove nuget package refs no longer required after move to net5.0

## 4.0.0

CommandDotNet.IoC.AutoFac targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.

## 3.0.0

### Nullable Reference Types

The library has been updated to support Nullable Reference Types

### Obsoletes

remove `useLegacyInjectDependenciesAttribute` option as it's no longer supported as of CommandDotNet 4.0.0

## 2.0.1

improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```
