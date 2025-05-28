using Cqrs.Shared.Infrastructure;
using Cqrs.Shared.Interfaces;
using Cqrs.Shared.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Physical.WriteApi.Data;
using Physical.WriteApi.Handlers;
using Physical.WriteApi.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<WriteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services
    .AddFluentValidationAutoValidation()     
    .AddFluentValidationClientsideAdapters(); 

builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderDtoValidator>();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<IRabbitMqSettings>(sp =>
    sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);
builder.Services.AddSingleton<IEventBus, RabbitMqEventBus>();


builder.Services.AddScoped<CreateOrderCommandHandler>();
builder.Services.AddMediatR(typeof(CreateOrderCommandHandler).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
