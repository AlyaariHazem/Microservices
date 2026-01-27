using Discount.Grpc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Application Services
// ----------------------
var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// ----------------------
// API / OpenAPI
// ----------------------
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Basket API",
        Version = "v1",
        Description = "Basket microservice using Carter, MediatR, Marten, Redis, and gRPC"
    });
});

// ----------------------
// Data Services
// ----------------------
builder.Services.AddCarter();
builder.Services.AddMapster();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    // opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
})
.UseLightweightSessions();

builder.Services.AddValidatorsFromAssembly(assembly);

// ----------------------
// Repositories
// ----------------------
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

// ----------------------
// Redis Cache
// ----------------------
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")!;
});

// ----------------------
// gRPC Client
// ----------------------
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(
        builder.Configuration.GetValue<string>("GrpcSettings:DiscountUrl")!
    );
});

// ----------------------
// Cross-Cutting Concerns
// ----------------------
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// ----------------------
// Middleware Pipeline
// ----------------------
app.UseExceptionHandler(_ => { });

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API v1");
        c.RoutePrefix = "swagger";
    });
}

// Carter Endpoints
app.MapCarter();

app.Run();
