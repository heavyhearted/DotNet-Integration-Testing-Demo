using SnowboardShop.Application;
using SnowboardShop.Application.Database;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddControllers();

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();

/* The Database Connection String is stored in appsettings.json for convenience of demo purposes.
In a real scenario, it should be stored securely, such as in user secrets, an environment variable, or a key vault.*/
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseRouteDebugger("/tools/route-debugger");

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();