using Microsoft.AspNetCore.Mvc;
using Physical.WriteApi.Commands;
using Physical.WriteApi.Commands.Handlers;

namespace Physical.WriteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderCommandHandler _handler;

    public OrdersController(CreateOrderCommandHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
    {
        var id = await _handler.Handle(command);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { Id = id }); // Для демонстрации
    }
}
