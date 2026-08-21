using Persistence;
using Persistence.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

// Crea la base de datos y aplica las migraciones pendientes al iniciar la API.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
