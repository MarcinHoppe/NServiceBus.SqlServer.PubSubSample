using System;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace Subscriber1Endpoint
{
    public class OrderAcceptedEventHandler : IHandleMessages<OrderAcceptedEvent>
    {
        public Task Handle(OrderAcceptedEvent message, IMessageHandlerContext context)
        {
            Console.WriteLine(
                $"Subscriber1Endpoint handles event from {context.MessageHeaders["NServiceBus.OriginatingEndpoint"]}");
            return Task.CompletedTask;
        }
    }
}