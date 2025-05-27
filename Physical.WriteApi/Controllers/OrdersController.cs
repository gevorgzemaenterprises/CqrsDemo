using MediatR;
using Microsoft.AspNetCore.Mvc;
using Physical.WriteApi.Commands;
using Physical.WriteApi.DTOs;

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
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
    {
        // валидация запускается автоматически, если включена ModelState
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new CreateOrderCommand
        {
            Description = dto.Description,
            Amount = dto.Amount
        };

        var id = await _mediator.Send(command);
        return Ok(id);
    }
}
