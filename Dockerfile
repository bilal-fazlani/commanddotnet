FROM squidfunk/mkdocs-material:5.1.6

COPY requirements-local.txt ./

RUN pip install -r requirements-local.txt