#!/bin/bash -e

# this script has been tested in git bash on windows.
# all usages in CommandDotNet were updated and tested with it.


# if you'd rather use VS, here's a guide to make it easier, and the regex to use
# how to find/replace using regex in VS: https://www.youtube.com/watch?v=6AsSfyHWWls

# LongName = "(.*)", ShortName = "(.)" => '$2', "$1"
# ShortName = "(.)", LongName = "(.*)" => '$1', "$2"
# LongName = null, ShortName = "(.)" => '$1', null
# ShortName = "(.)", LongName = null => '$1', null
# ShortName = "(.)" => '$1'
# LongName = "(.*)" => "$1"
# LongName = null => (string)null


find -type f -name "*.cs" -not -path '*/\.git/*' ! -path '*/obj/*' ! -path '*/bin/*' -print0 | while IFS= read -r -d $'\0' file; do
#    echo "$file";
    sed -br -i 's/LongName = "(.*)", ShortName = "(.)"/'\''\2'\'', "\1"/g' "$file"
    sed -br -i 's/ShortName = "(.)", LongName = "(.*)"/'\''\1'\'', "\2"/g' "$file"
    sed -br -i 's/LongName = null, ShortName = "(.)""/'\''\1'\'', null/g' "$file"
    sed -br -i 's/ShortName = "(.)", LongName = null"/'\''\1'\'', null/g' "$file"
    sed -br -i 's/ShortName = "(.)"/'\''\1'\''/g' "$file"
    sed -br -i 's/LongName = "(.*)"/"\1"/g' "$file"
    sed -br -i 's/LongName = null/(string)null/g' "$file"
    sed -br -i 's/\[(Command|Operand)\(Name = "(.*)"/\[\1\("\2"/g' "$file"
    sed -br -i 's/\[SubCommand\]/\[Subcommand\]/g' "$file"
done