#!/bin/bash -e

echo "installing tools"
pip install mkdocs mkdocs-material pygments pymdown-extensions recommonmark
echo "installed"

git worktree add site gh-pages

echo "generating new documentation"
mkdocs build
echo "documentation generated"

echo "commit pages"
git config --global user.email "travis@travis-ci.com"
git config --global user.name "Travis CI"
git config --global push.default current
cd site
git add --all
git commit -m "${TRAVIS_COMMIT_MESSAGE}"
echo "committed pages"

echo "publishing to github"
git push https://${GITHUB_TOKEN}@github.com/bilal-fazlani/commanddotnet.git
echo "published"

cd ..
git worktree remove site
