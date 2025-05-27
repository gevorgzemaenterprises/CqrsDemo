using Physical.WriteApi.Data;
using Physical.WriteApi.Models;

namespace Physical.WriteApi.Commands.Handlers;

public class CreateOrderCommandHandler
{
    private readonly WriteDbContext _context;

    public CreateOrderCommandHandler(WriteDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateOrderCommand command)
    {
        var order = new Order
        {
            Description = command.Description,
            Amount = command.Amount,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order.Id;
    }
}
