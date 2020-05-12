FROM squidfunk/mkdocs-material:4.6.0

COPY requirements-local.txt ./

RUN pip install -r requirements-local.txt
