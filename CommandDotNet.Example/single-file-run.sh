#!/bin/bash -e

pushd bin/Debug/net7.0/osx-arm64/publish/
# CommandDotNet.Example.exe $1 $2 $3 $4 $5 $6 $7 $8 $9 ${10} ${11} ${12} ${13}
./CommandDotNet.Example $1 $2 $3 $4 $5 $6 $7 $8 $9 ${10} ${11} ${12} ${13}
popd