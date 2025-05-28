using MediatR;
using Microsoft.EntityFrameworkCore;
using Physical.ReadApi.Data;
using Physical.ReadApi.DTOs;
using Physical.ReadApi.Queries;

namespace Physical.ReadApi.Handlers
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly ReadDbContext _context;

        public GetOrderByIdQueryHandler(ReadDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            return order == null ? null : new OrderDto
            {
                Id = order.Id,
                Description = order.Description,
                Amount = order.Amount,
                CreatedAt = order.CreatedAt
            };
        }
    }
}
