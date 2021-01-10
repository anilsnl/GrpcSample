# GrpcSample
This sample app contains all types of gRPC services. 

In this sample app .NET 5 is used in botch client and server application. The following aspects is included in the app...

1. gRPC Authentication - JWT is preferred as authorization method.
2. gRPC Exception handling.
4. All types of gRPC communication witch are unary, clientside & serverside streaming, bidirectional.

## Unary
Unary is basically classic REST request. You send a single request and get a single response. In this example I use simple api that take a country code and returns some info about that country.

## Serverside Streaming
Serverside streaming is like live stock exchange system. You open the page with a single request and data updates without refreshing the page.

You can see this as a simple coin exchange app in the code.

## Clientside Streaming
In clientside streaming client sends data part by part and server returns just a response.

In the code, you can see simple logger service to read log from client.

## Bidirectional Streaming
With the bidirectional streaming, client can getting data while sending an other data to the server. 

One of the good example is a simple chat application. While messaging in a chat app, You can get message while typing a reply in a connection
