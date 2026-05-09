namespace Shared.Contracts
{
    public class OrderCreatedEvent
    {
        public long OrderId { get; set; }

        public long ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
