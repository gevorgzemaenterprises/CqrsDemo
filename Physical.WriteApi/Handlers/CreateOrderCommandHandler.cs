using MediatR;
using Physical.WriteApi.Commands;
using Physical.WriteApi.Data;
using Physical.WriteApi.Entities;

namespace Physical.WriteApi.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly WriteDbContext _context;

        public CreateOrderCommandHandler(WriteDbContext context)
        {
            _context = context;
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

            return order.Id;
        }
    }
}
