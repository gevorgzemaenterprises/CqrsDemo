using MediatR;
using Cqrs.LogicalApi.Infrastructure;
using Cqrs.LogicalApi.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Cqrs.LogicalApi.Application.Queries {
    public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Order> {
        private readonly AppDbContext _db;

        public GetOrderByIdHandler(AppDbContext db) => _db = db;

        public async Task<Order> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken) {
            return await _db.Orders.FindAsync(new object[] { request.Id }, cancellationToken);
        }
    }
}
