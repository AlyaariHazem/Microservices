var builder = WebApplication.CreateBuilder(args);

//Add Services to Controllers.

var app = builder.Build();

//Configure the HTTP request Pipelines.

app.Run();
