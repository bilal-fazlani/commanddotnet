#!/bin/bash -e

parseTravisTag () {
  if [[ $TRAVIS_TAG =~ (.+)_(.+) ]]
  then
    PROJECT_NAME=${BASH_REMATCH[1]}
    DEPLOYMENT_VERSION=${BASH_REMATCH[2]}
    echo "TRAVIS_TAG resolved to project $PROJECT_NAME & version $DEPLOYMENT_VERSION"
  else
    >&2 echo "failed to parse TRAVIS_TAG of value '$TRAVIS_TAG'"
    exit 1
  fi
}

parseTravisTag

# PACKAGE
dotnet pack \
-o output \
-c Release \
$PROJECT_NAME/$PROJECT_NAME.csproj \
/p:Version=$DEPLOYMENT_VERSION

# TESTING SCRIPT
echo "output directory:"
ls output/

# #PUBLISH TO NUGET
# dotnet nuget push -s https://api.nuget.org/v3/index.json -k $NUGET_API_KEY_COMMANDDOTNET output/$PROJECT_NAME.$DEPLOYMENT_VERSION.nupkg