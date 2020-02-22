FROM python:3.7

COPY requirements.txt ./

RUN pip3 install -r requirements.txt

ENTRYPOINT ["mkdocs"]

EXPOSE 8000

CMD ["serve", "--dev-addr=0.0.0.0:8000"]
