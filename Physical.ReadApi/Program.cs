using Cqrs.Shared.Events;
using Cqrs.Shared.Infrastructure;
using Cqrs.Shared.Interfaces;
using Cqrs.Shared.Settings;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Physical.ReadApi.Data;
using Physical.ReadApi.EventHandlers;
using Physical.ReadApi.Handlers;
using Physical.ReadApi.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF Core
builder.Services.AddDbContext<ReadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MediatR
builder.Services.AddMediatR(typeof(GetOrderByIdQueryHandler).Assembly);

//  Add RabbitMQ settings + event bus
builder.Services.AddSingleton<IRabbitMqSettings, RabbitMqSettings>();
builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

//  Add event handler
builder.Services.AddScoped<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();

var app = builder.Build();

// Subscribe to RabbitMQ event bus for OrderCreatedEvent.
// When the event is received, resolve the corresponding event handler from a new service scope
// and invoke its HandleAsync method to process the event.
using (var scope = app.Services.CreateScope())
{
    var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();

    bus.Subscribe<OrderCreatedEvent>(async (@event) =>
    {
        using var innerScope = app.Services.CreateScope();
        var handler = innerScope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<OrderCreatedEvent>>();
        await handler.HandleAsync(@event);
    });
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
