# Table Tennis Tournament (T3)

This project contains software for creating a server and client to manage a table tennis tournament.

> ⚠️ **Important**<br>
> A lot of the code is still explorative coding, and a lot can change between now and the final version.

## Goal

1. Create a flexible software that can run the majority of table tennis tournaments since there are a lot of [variations](docs/table-tennis-variations.md).
2. Allow anyone to create their own clients.

## Quick start

The following quick start is intended for developers and anyone who wants to run the software quickly.

### Requirements

Have the following tools installed:
* dotnet (I have version 7)
* nodejs (I have version 19)
* docker (I have version 23)

```sh
# Checkout the project in "t3" folder
git clone https://github.com/NTTB/table-tennis-tournament t3

# Enter the directory of the typescript client
cd t3/src/clients/ts

# Install the dependencies, create a build and make a local link
npm install
npm run build
npm link # To undo this use `npm unlink` in this directory

# Enter the source directory of the web client
cd ../../server/T3.Web/ClientApp/
# Install the dependencies and link to the local version of the t3-api-client
npm i
npm link @nttb/t3-api-client

# Navigate to the server
cd ../..
# Start the mongo-database
docker compose up -d
# Run the server (which also host the client)
dotnet run --project T3.Web
```

Then look for which https port it is listening to and navigate to that. This will result in the server starting the webclient.

Create an account, login, say you want to regenerate the keys and then navigate to sets.