using NServiceBus;

namespace Messages
{
    public class OrderAcceptedEvent : IEvent
    {
        public string OrderId { get; set; }
        public string CustomerId { get; set; }
    }
}