namespace Cqrs.Shared.Events
{
    public class OrderCreatedEvent
    {
        public int Id { get; set; }
        public string Description { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
