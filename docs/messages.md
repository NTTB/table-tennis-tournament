# Messages

By default we wish to prevent data being polled from the server. So anything that for some reason might require live-updates is pushed from the server to the client.
This is done using [SignalR](https://dotnet.microsoft.com/en-us/apps/aspnet/signalr).

Each message is also signed by the client. Every client generates a public-private key and uploads the public key to the server. Any messages the client will send will be signed with the private key and the server can, but is not required, use that to verify the signature.

The primary function of the server is to distribute data and not to make assumptions about that.