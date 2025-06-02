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

// ? Capture root service provider ONCE
var serviceProvider = app.Services;

// ? Subscribe to events using a scoped handler
using (var scope = serviceProvider.CreateScope())
{
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

    eventBus.Subscribe<OrderCreatedEvent>(async @event =>
    {
        using var innerScope = serviceProvider.CreateScope();
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
