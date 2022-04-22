# syntax=docker/dockerfile:1
FROM alpine:3.15 AS builder 

RUN apk update && \
    apk add git make cmake g++ libbson-static musl-dev libc-dev openssl openssl-dev py3-pip icu-dev bash nano coreutils

RUN /bin/bash -c "cd /tmp ; \
 git clone --branch 1.21.1 https://github.com/mongodb/mongo-c-driver ;\
 cd mongo-c-driver/ ;\
 cmake -DENABLE_MONGOC=OFF . ;\
 make -j8 install ;\
 cd .. ;\
 git clone --branch 1.3.2 https://github.com/mongodb/libmongocrypt ;\
 cd libmongocrypt/ ;\
 cmake . ;\
 make install"


FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.15

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
RUN apk update

RUN mkdir -p /code/app/out
WORKDIR /code/app
COPY ./csharp /code/app
COPY --from=builder /usr/local/lib/libmongocrypt.so.0.0.0 /code/app/out/libmongocrypt.so

ENTRYPOINT [ "/bin/sh", "-c" ]
