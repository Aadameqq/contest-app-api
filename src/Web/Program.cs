using Core;
using Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.SetUpOptions();
builder.SetUpStatusCodes();
builder.Services.SetUpCore();
builder.Services.AddControllers();

builder.Services.SetUpOpenApi();
builder.Services.SetUpAuth();

var app = builder.Build();

app.UseStatusCodes();
app.UseOpenApi();

app.UseHttpsRedirection();

app.UseAuth();
app.MapControllers();

app.Run();
