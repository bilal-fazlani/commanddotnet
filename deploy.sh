#!/bin/bash -e

dotnet pack \
-o output \
-c Release \
CommandDotNet/CommandDotNet.csproj \
/p:Version=$TRAVIS_TAG


