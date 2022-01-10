#!/bin/bash -e

dotnet publish -r win-x64 --sc -p:PublishSingleFile=true

# to enable debugging
cp bin/Debug/net5.0/win-x64/mscordbi.dll bin/Debug/net5.0/win-x64/publish/mscordbi.dll

echo
echo files
echo

ls bin/Debug/net5.0/win-x64/publish