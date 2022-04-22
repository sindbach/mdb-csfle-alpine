
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

```s
docker run --rm -e MONGODB_URI=<MONGODB ATLAS URI> mdb-csfle-alpine "dotnet publish -o out -c release-standalone -r linux-musl-x64 --self-contained; dotnet /code/app/out/app.dll"
```

Output: 

```s
Microsoft (R) Build Engine version 17.1.1+a02f73656 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored /code/app/app.csproj (in 15.05 sec).
  app -> /code/app/bin/release-standalone/net6.0/linux-musl-x64/app.dll
  app -> /code/app/out/
Original string 123456789.
Encrypted value Encrypted:0x0157ebca8bec814fe6970ea78d977c8dad0212659ca8a676f0448461524134a032b49c623b15fc17a5d7fc3463458bb69e11e91bafc23aee07389b5ab6f670762caabbc7a77c661e5c1c53dc97e1d608c4ba.
Decrypted document { "_id" : ObjectId("62625f5ced0a774db45738b8"), "encryptedField" : "123456789" }.
```
