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
using Physical.ReadApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddScoped<OrderCreatedEventHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add EF Core
builder.Services.AddDbContext<ReadDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MediatR
builder.Services.AddMediatR(typeof(GetOrderByIdQueryHandler).Assembly);

// Add RabbitMQ
builder.Services.AddSingleton<IRabbitMqSettings, RabbitMqSettings>();
builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();

// Add event handlers
builder.Services.AddScoped<IIntegrationEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();

var app = builder.Build();

// ?? ?????????? ???????? ?? ??????? (????? ??????????)
app.UseEventBusSubscriptions();

// Swagger ? ?????????
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
