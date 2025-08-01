
var builder = WebApplication.CreateBuilder(args);

// Add Services to Controllers
builder.Services.AddCarter();
builder.Services.AddMapster();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
var app = builder.Build();

// Configure the HTTP request Pipelines
app.MapCarter();

app.Run();
