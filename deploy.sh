#!/bin/bash -e

#PACKAGE
dotnet pack \
-o output \
-c Release \
CommandDotNet/CommandDotNet.csproj \
/p:Version=$TRAVIS_TAG

#PUBLISH TO NUGET
dotnet nuget push -k $NUGET_API_KEY_COMMANDDOTNET CommandDotNet.$TRAVIS_TAG.nupkg