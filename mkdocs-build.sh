#!/bin/bash -e

docker build --no-cache -t mkdocs .

# use this line on mac/linux.
#docker run --rm --name mkdocs-material -it -v ${PWD}:/docs mkdocs build --strict

# use this line on windows to escape the path conversion on windows.  
# https://stackoverflow.com/questions/50608301/docker-mounted-volume-adds-c-to-end-of-windows-path-when-translating-from-linux
docker run --rm --name mkdocs-material -it -v /${PWD}:/docs mkdocs build --strict