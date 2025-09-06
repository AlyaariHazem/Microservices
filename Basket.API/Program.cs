
using Discount.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add Services to Controllers

//Application services
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

//Data Services
builder.Services.AddCarter();
builder.Services.AddMapster();
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    //opts.Schema.For<ShoppingCart>().IDentity(x=>x.UserName);
}).UseLightweightSessions();
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")!;
    //options.InstanceName = "BasketAPI:";
});
// gRPC Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration.GetValue<string>("GrpcSettings:DiscountUrl")!);
});
//Cross-cutting concerns
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
    
var app = builder.Build();

// Configure the HTTP request Pipelines
app.MapCarter();
app.Run();
