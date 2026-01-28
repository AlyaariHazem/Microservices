using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Services to Controllers
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddValidatorsFromAssembly(typeof(BuildingBlocks.Querying.Validation.QueryParametersValidator).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1",
        Description = "Catalog microservice using Carter, MediatR, and Marten"
    });
});

builder.Services.AddCarter();
builder.Services.AddMapster();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogIntialData>();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request Pipelines
app.UseExceptionHandler(_ => { });

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapCarter();

app.Run();
