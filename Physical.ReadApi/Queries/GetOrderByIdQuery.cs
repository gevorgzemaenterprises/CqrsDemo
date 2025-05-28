using MediatR;
using Physical.ReadApi.DTOs;

namespace Physical.ReadApi.Queries
{
    public record GetOrderByIdQuery(int Id) : IRequest<OrderDto>;
}
