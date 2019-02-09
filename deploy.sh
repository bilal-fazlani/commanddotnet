#!/bin/bash -e

#PACKAGE
dotnet pack \
-o output \
-c Release \
CommandDotNet/CommandDotNet.csproj \
/p:Version=$TRAVIS_TAG

#PUBLISH TO NUGET
dotnet nuget push -s https://api.nuget.org/v3/index.json -k $NUGET_API_KEY_COMMANDDOTNET output/CommandDotNet.$TRAVIS_TAG.nupkg