#!/usr/bin/env bash -e

# dotnet pack \
# -o output \
# -c Release \
# CommandDotNet/CommandDotNet.csproj \
# /p:Version=$TRAVIS_TAG

echo "COMMAND_DOT_NET VERSION $TRAVIS_TAG NUGET PACKAGE CREATED"