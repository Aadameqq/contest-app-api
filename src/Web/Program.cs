using Core;
using Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.SetUpOptions();
builder.Services.SetUpCore();
builder.Services.AddControllers();

builder.Services.SetUpOpenApi();

var app = builder.Build();

app.UseOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
