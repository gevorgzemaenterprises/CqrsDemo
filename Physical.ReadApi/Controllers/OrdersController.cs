using MediatR;
using Microsoft.AspNetCore.Mvc;
using Physical.ReadApi.Queries;

namespace Physical.ReadApi.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery(id));
            return result != null ? Ok(result) : NotFound();
        }
    }
}
