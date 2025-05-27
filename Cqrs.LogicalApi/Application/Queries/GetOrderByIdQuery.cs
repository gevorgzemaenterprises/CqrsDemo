using MediatR;
using Cqrs.LogicalApi.Domain;
using System;

namespace Cqrs.LogicalApi.Application.Queries {
    public class GetOrderByIdQuery : IRequest<Order> {
        public Guid Id { get; set; }
    }
}
