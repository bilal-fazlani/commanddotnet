
# install the tool first
# dotnet tool install -g MarkdownSnippets.Tool

# extract snipptes from code and insert to docs
mdsnippets -c InPlaceOverwrite --url-prefix "https://github.com/bilal-fazlani/commanddotnet/blob/master"

# mdsnippets uses ```cs for files and pygments expects ```c#, so fix the links
find ./docs/ \( -type d -name .git -prune \) -o -type f -print0 | xargs -0 sed -b -i 's/```cs/```c#/g'