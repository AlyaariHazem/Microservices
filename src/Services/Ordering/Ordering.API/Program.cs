using Ordering.API;
using Ordering.Application;
using Ordering.Infrastracure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices();

var app = builder.Build();


//configure the HTTP request pipeline

app.Run();
