# How to configure NServiceBus pub/sub using the SQL Server transport?

This sample illustrates how to configure NServiceBus routing to enable pub/sub with the SQL Server transport.

## How to run the sample?

Set `PublisherEndpoint`, `Subscriber1Endpoint`, and `Subscriber2Endpoint` projects as startup projects. This is all that is required to run it.


## Publisher

The publisher does not require any special configuration when it comes to routing to support pub/sub:

```csharp
var endpointConfiguration = new EndpointConfiguration("PublisherEndpoint");

endpointConfiguration.UseTransport<SqlServerTransport>()
    .ConnectionString(@"Data Source=.\SQLEXPRESS; Initial Catalog=nservicebus;Integrated Security=True");

endpointConfiguration.UsePersistence<InMemoryPersistence>();
endpointConfiguration.EnableInstallers();
endpointConfiguration.SendFailedMessagesTo("error");

var endpointInstance = await Endpoint.Start(endpointConfiguration);
```

When the publisher endpoint is running, it publishes a single event:

```csharp
await endpointInstance.Publish<OrderAcceptedEvent>(e =>
{
    e.OrderId = Guid.NewGuid().ToString();
    e.CustomerId = Guid.NewGuid().ToString();
});
```


## Subscribers

Each subscriber needs to configure routing on a transport level so that NServiceBus knows where to send the _subscription message_:

```csharp
var endpointConfiguration = new EndpointConfiguration("Subscriber1Endpoint");

var transport = endpointConfiguration.UseTransport<SqlServerTransport>();

transport.ConnectionString(
    @"Data Source=.\SQLEXPRESS; Initial Catalog=nservicebus;Integrated Security=True");

// Configure the pub/sub routing.
transport.Routing()
    .RegisterPublisher(typeof(OrderAcceptedEvent), "PublisherEndpoint");

endpointConfiguration.UsePersistence<InMemoryPersistence>();
endpointConfiguration.EnableInstallers();
endpointConfiguration.SendFailedMessagesTo("error");

var endpointInstance = await Endpoint.Start(endpointConfiguration);
```

When publisher endpoint publishes an event, each of the subscribers has a handle to process it:

```csharp
public class OrderAcceptedEventHandler : IHandleMessages<OrderAcceptedEvent>
{
    public Task Handle(OrderAcceptedEvent message, IMessageHandlerContext context)
    {
        Console.WriteLine(
            $"Subscriber1Endpoint handles event from {context.MessageHeaders["NServiceBus.OriginatingEndpoint"]}");
        return Task.CompletedTask;
    }
}
```

## Connection string

Running the sample requires that each endpoint has a valid SQL Server connection string. The default connection string is:

```
Data Source=.\SQLEXPRESS; Initial Catalog=nservicebus;Integrated Security=True
```

For other connection parameters, the transport connection string needs to be configured in each endpoint:

```csharp
var connectionString = "...";
endpointConfiguration.UseTransport<SqlServerTransport>()
    .ConnectionString(connectionString);
```
