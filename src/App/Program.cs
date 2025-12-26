using System.Text.Json.Serialization;
using App.Common;
using App.Configurations;
using App.Features;
using App.Identity;
using App.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.SetUpOptions();
builder.SetUpStatusCodes();
builder.Services.SetUpCommon();
builder.Services.SetUpFeatures();
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
