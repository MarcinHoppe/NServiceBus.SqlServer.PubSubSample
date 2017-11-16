using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace Subscriber2Endpoint
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("Subscriber2Endpoint");

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

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            await endpointInstance.Stop();
        }
    }
}