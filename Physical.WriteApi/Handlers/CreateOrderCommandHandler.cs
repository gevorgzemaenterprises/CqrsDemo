using Cqrs.Shared.Events;
using Cqrs.Shared.Interfaces;
using MediatR;
using Physical.WriteApi.Commands;
using Physical.WriteApi.Data;
using Physical.WriteApi.Entities;

namespace Physical.WriteApi.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly WriteDbContext _context;
        private readonly IEventBus _eventBus;

        public CreateOrderCommandHandler(WriteDbContext context, IEventBus eventBus)
        {
            _context = context;
            _eventBus = eventBus;
        }

        public async Task<int> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                Description = command.Description,
                Amount = command.Amount,
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            var @event = new OrderCreatedEvent
            {
                OrderId = order.Id,
                Description = order.Description,
                Amount = order.Amount,
                CreatedAt = order.CreatedAt
            };

            _eventBus.Publish(@event);

            return order.Id;
        }
    }

}
