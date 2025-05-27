using Microsoft.EntityFrameworkCore;
using Physical.WriteApi.Commands.Handlers;
using Physical.WriteApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<WriteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<CreateOrderCommandHandler>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
