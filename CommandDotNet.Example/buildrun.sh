#!/bin/bash -e
dotnet build
clear
dotnet bin/Debug/netcoreapp2.0/CommandDotNet.Example.dll $1 $2 $3 $4 $5 $6 $7