using MediatR;
using Cqrs.LogicalApi.Infrastructure;
using Cqrs.LogicalApi.Domain;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Cqrs.LogicalApi.Application.Commands {
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Guid> {
        private readonly AppDbContext _db;

        public CreateOrderHandler(AppDbContext db) => _db = db;

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken) {
            var order = new Order {
                Id = Guid.NewGuid(),
                ProductName = request.ProductName,
                Quantity = request.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync(cancellationToken);

            return order.Id;
        }
    }
}
