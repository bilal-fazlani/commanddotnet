# CommandDotNet.NameCasing

## 6.1.0

* Accidental increment. No features or fixes. Apologies for the noise.

## 5.0.0

* add support for net9
* drop support for net6 and net7

## 4.0.2

* support dotnet 7

## 4.0.1

* update to dotnet 6

## 3.0.1

remove nuget package refs no longer required after move to net5.0

## 3.0.0

CommandDotNet.NameCasing targets net5.0 instead of netstandard2.0.  This will allow us to take advantage of new framework features.
We're holding off on net6.0 at the moment because it's new enough many companies will not be able to adopt it yet.

## 2.0.0

## 1.0.1

improve stack traces with AppOutputBase and PathMap project settings
```
<AppOutputBase>$(MSBuildProjectDirectory)\</AppOutputBase>
<PathMap>$(AppOutputBase)=CommandDotNet/</PathMap>
```
