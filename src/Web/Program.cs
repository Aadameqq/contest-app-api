using System.Text.Json.Serialization;
using Core;
using Web.Configurations;
using Web.Identity;
using Web.OpenApi;

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

public partial class Program { }
