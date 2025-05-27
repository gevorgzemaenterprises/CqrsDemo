using MediatR;
using System;

namespace Cqrs.LogicalApi.Application.Commands {
    public class CreateOrderCommand : IRequest<Guid> {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
