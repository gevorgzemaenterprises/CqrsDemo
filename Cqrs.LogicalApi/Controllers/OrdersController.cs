using Microsoft.AspNetCore.Mvc;
using MediatR;
using Cqrs.LogicalApi.Application.Commands;
using Cqrs.LogicalApi.Application.Queries;
using System.Threading.Tasks;
using System;

namespace Cqrs.LogicalApi.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command) {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id) {
            var result = await _mediator.Send(new GetOrderByIdQuery { Id = id });
            return result is null ? NotFound() : Ok(result);
        }
    }
}
