FROM mkdoc

WORKDIR /project

COPY docs ./docs
COPY mkdocs.yml ./