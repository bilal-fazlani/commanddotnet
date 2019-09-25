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

updateProjectRefsInNuspec () {  
  ## get list of projects in solution referenced by PROJECT_NAME
  ## after grep, lines will look like ..\CommandDotNet\CommandDotNet.csproj
  ## sed removes start ..\
  dotnet list ../../$PROJECT_FILE reference | grep csproj | sed 's/^...//' | while read -r projectRefFile; do    
    if [[ $projectRefFile =~ (.+)[/\\](.+) ]]
    then
      projectRefName=${BASH_REMATCH[1]}
    else
      >&2 echo "failed to parse projectRefFile of value '$projectRefFile'"
      exit 1
    fi

    tagName=`git describe --tags --match "$projectRefName_*"`

    if [[ $tagName =~ (.+)_(.+)-(.+)-(.+) ]]
    then
      projectRefVersion=${BASH_REMATCH[2]}
      echo "projectRefVersion=$projectRefVersion"
    else
      >&2 echo "failed to parse projectRefVersion of value '$tagName' for $projectRefName"
      exit 1
    fi

    echo "projectRefFile=$projectRefFile projectRefName=$projectRefName projectRefVersion=$projectRefVersion tagName=$tagName"
    
    # dotnet pack has a bug: https://github.com/NuGet/Home/issues/7328
    # - project reference versions set to pack version 
    # so we'll check DEPLOYMENT_VERSION and also 1.0.0 so this still works when the bug is fixed     
    sed -i "s/id=\"$projectRefName\" version=\"1.0.0\"/id=\"$projectRefName\" version=\"$projectRefVersion\"/" $NUSPEC_FILE
    sed -i "s/id=\"$projectRefName\" version=\"$DEPLOYMENT_VERSION\"/id=\"$projectRefName\" version=\"$projectRefVersion\"/" $NUSPEC_FILE
  done
}

parseTravisTag

PROJECT_FILE=$PROJECT_NAME/$PROJECT_NAME.csproj
NUPKG_FILE=$PROJECT_NAME.$DEPLOYMENT_VERSION.nupkg
NUSPEC_FILE=$PROJECT_NAME.nuspec

# PACKAGE
dotnet pack \
-o output \
-c Release \
$PROJECT_FILE \
-p:Version=$DEPLOYMENT_VERSION \
--no-restore \
#-v:diag

# update nuspec with correct versions of referenced projects  
cd $PROJECT_NAME/output
unzip $NUPKG_FILE $NUSPEC_FILE

updateProjectRefsInNuspec

zip $NUPKG_FILE $NUSPEC_FILE
rm $NUSPEC_FILE
cd ../..

# PUBLISH TO NUGET
dotnet nuget push -s https://api.nuget.org/v3/index.json -k $NUGET_API_KEY_COMMANDDOTNET CommandDotNet/output/$PROJECT_NAME.$DEPLOYMENT_VERSION.nupkg