FROM squidfunk/mkdocs-material:4.6.0

COPY requirements.txt ./

RUN pip install -r requirements.txt