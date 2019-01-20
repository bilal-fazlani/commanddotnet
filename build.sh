#!/usr/bin/env bash -e

dotnet pack -o output -c Release --include-symbols -p:SymbolPackageFormat=snupkg --include-source CommandDotNet/CommandDotNet.csproj /p:Version=%APPVEYOR_BUILD_VERSION%