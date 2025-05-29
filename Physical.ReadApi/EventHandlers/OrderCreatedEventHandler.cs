using Cqrs.Shared.Events;
using Cqrs.Shared.Interfaces;
using Physical.ReadApi.Data;
using Physical.ReadApi.Models;

namespace Physical.ReadApi.EventHandlers
{
    public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
    {
        private readonly ReadDbContext _context;

        public OrderCreatedEventHandler(ReadDbContext context)
        {
            _context = context;
        }

        public async Task HandleAsync(OrderCreatedEvent @event)
        {
            var order = new OrderReadModel
            {
                Id = @event.OrderId,
                Description = @event.Description,
                Amount = @event.Amount,
                CreatedAt = @event.CreatedAt
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }
    }
}
