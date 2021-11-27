#!/bin/bash -e

rename() {
    echo "$1"
    sed -br -i 's///g' "$1"
    sed -br -i 's///g' "$1"
    sed -br -i 's///g' "$1"
}

find -type f -path '*/docs/*' -name "*.md" -print0 | while IFS= read -r -d $'\0' file; do
    rename "$file";
done


find -type f -path '*/CommandDotNet.DocExamples/*' -name "*.cs" -print0 | while IFS= read -r -d $'\0' file; do
    rename "$file";
done