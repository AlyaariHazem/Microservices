var builder = WebApplication.CreateBuilder(args);

//Add Services to Controllers.
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var app = builder.Build();

//Configure the HTTP request Pipelines.
app.MapCarter();

app.Run();
