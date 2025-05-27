using MediatR;
using Microsoft.AspNetCore.Mvc;
using Physical.WriteApi.Commands;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(id);
    }
}
