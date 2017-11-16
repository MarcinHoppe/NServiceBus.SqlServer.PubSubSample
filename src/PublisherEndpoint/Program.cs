using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace PublisherEndpoint
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("PublisherEndpoint");

            endpointConfiguration.UseTransport<SqlServerTransport>()
                .ConnectionString(@"Data Source=.\SQLEXPRESS; Initial Catalog=nservicebus;Integrated Security=True");

            endpointConfiguration.UsePersistence<InMemoryPersistence>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.SendFailedMessagesTo("error");

            var endpointInstance = await Endpoint.Start(endpointConfiguration);

            Console.WriteLine("Press any key to publish a message.");
            Console.ReadKey();

            await endpointInstance.Publish<OrderAcceptedEvent>(e =>
            {
                e.OrderId = Guid.NewGuid().ToString();
                e.CustomerId = Guid.NewGuid().ToString();
            });

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            await endpointInstance.Stop();
        }
    }
}