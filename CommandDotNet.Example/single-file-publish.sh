#!/bin/bash -e

# dotnet publish -r win-x64 -f net7.0 --self-contained -p:PublishSingleFile=true
dotnet publish -r osx-arm64 -f net7.0 --self-contained -c release -p:PublishSingleFile=true

# to enable debugging
# cp bin/Debug/net7.0/win-x64/mscordbi.dll bin/Debug/net7.0/win-x64/publish/mscordbi.dll
cp bin/Debug/net7.0/osx-arm64/mscordbi.dll bin/Debug/net7.0/osx-arm64/publish/mscordbi.dll

echo
echo files
echo

#ls bin/Debug/net7.0/win-x64/publish
ls bin/Debug/net7.0/osx-arm64/publish