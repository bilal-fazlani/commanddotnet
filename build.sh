#! /bin/bash -e

dotnet test

dotnet pack \
-o output \
-c Release \
--include-symbols \
-p:SymbolPackageFormat=snupkg \
--include-source \
CommandDotNet/CommandDotNet.csproj \
/p:Version=1.0.0-SNAPSHOT