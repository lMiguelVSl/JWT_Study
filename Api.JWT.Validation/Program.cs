var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/protected", () => "Hello World, protected").RequireAuthorization();

app.Run();