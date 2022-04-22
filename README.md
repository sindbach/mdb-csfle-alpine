
# MongoDB CSFLE on Alpine - Example

Originally from: https://gist.github.com/kennwhite/0b3df24191eba4d04969646aca1cfeb5

## Overview 

This example is to show the execution of MongoDB Client-Side Field Level Encryption (CSFLE) with Alpine Docker using .NET/C# as the client application.

## Build Step

From the root directory of this repo, build a Docker image using the following:

```s
docker build . -t mdb-csfle-alpine
```

The above command should build a Docker image with a tag name `mdb-csfle-alpine`


## Execution Step

Please substitute `<MONGODB CONN URI>` below with a valid MongoDB connection string. 

### Runtime: linux-musl-x64 
```s
docker run --rm -e MONGODB_URI=<MONGODB CONN URI> mdb-csfle-alpine "dotnet publish -o out-standalone -c release-standalone -r linux-musl-x64 --self-contained; cp -v /code/app/libmongocrypt.so /code/app/out-standalone/; dotnet /code/app/out-standalone/app.dll"
```

Output: 

```s
Microsoft (R) Build Engine version 17.1.1+a02f73656 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored /code/app/app.csproj (in 13.91 sec).
  app -> /code/app/bin/release-standalone/net6.0/linux-musl-x64/app.dll
  app -> /code/app/out-standalone/
'/code/app/libmongocrypt.so' -> '/code/app/out-standalone/libmongocrypt.so'
Original string 123456789.
Encrypted value Encrypted:0x012a927d4114e54e32abdfa4bfc756a1d30220b842942d3c2e5cc13060a4b2d0d0560877d24bfbd8f09ab72cbf8700b74c52585ba0023e2a54261ce2e288f917ba859700db9a7ffee452fcf08c414ddfc3cc.
Decrypted document { "_id" : ObjectId("6262702c702aaa2c316fd64c"), "encryptedField" : "123456789" }.
```

### Runtime: native 

```s
docker run --rm -e MONGODB_URI=<MONGODB CONN URI> mdb-csfle-alpine "dotnet publish -o out -c release; cp -v /code/app/libmongocrypt.so /code/app/out/runtimes/linux/native/; dotnet /code/app/out/app.dll"
```

Output: 

```s
Microsoft (R) Build Engine version 17.1.1+a02f73656 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored /code/app/app.csproj (in 5.52 sec).
  app -> /code/app/bin/release/net6.0/app.dll
  app -> /code/app/out/
'/code/app/libmongocrypt.so' -> '/code/app/out/runtimes/linux/native/libmongocrypt.so'
Original string 123456789.
Encrypted value Encrypted:0x01cd3925ba2a8240aaacd30fc10a8043a30289483a21728a766760fe1310446f3ed8a4ecb079198680607a941ac4e03a6de72e0b4cbd814c5816c18e1ef8f9ca433e25511ae92fd9738658407b68f21d40c2.
Decrypted document { "_id" : ObjectId("6262709fa625de28bf446848"), "encryptedField" : "123456789" }.
```
