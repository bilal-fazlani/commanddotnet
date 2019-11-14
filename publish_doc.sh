#!/bin/bash -e

git_config_travis() {
  echo "configuring git for travis"
  git config --global user.email "travis@travis-ci.com"
  git config --global user.name "Travis CI"
}

git_add_site() {
  echo "adding worktree 'site'"
  git worktree add site gh-pages
}

git_remove_site() {
  # skip in travis. this fails and is not necessary there
  if [ -z "${TRAVIS_COMMIT_MESSAGE}"]
  then
    echo "removing worktree 'site'"
    git worktree remove site
  fi
}

mkdocks_build() {
  chmod 777 ./mkdocs-build.sh
  ./mkdocs-build.sh
}

git_commit_site() {
  echo "commit pages"
  cd site
  git add --all
  git commit -m "$COMMIT_MSG"
  cd ..
  echo "committed pages"
}

git_push_site() {  
  echo "publishing to github"
  cd site
  git push https://${GITHUB_TOKEN}@github.com/bilal-fazlani/commanddotnet.git gh-pages
  cd ..
  echo "published"
}

COMMIT_MSG="${TRAVIS_COMMIT_MESSAGE}"

if [ -z "$COMMIT_MSG" ]
then
  COMMIT_MSG="$1"
  if [ -z "$COMMIT_MSG" ]
  then
    echo "commit message required"
    echo "usage: publish_doc.sh 'commit message'"
    exit 1
  fi
else
  git_config_travis
fi
git_add_site
mkdocks_build
git_commit_site
#git_push_site
git_remove_site
