using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ConfigurableWorkflowEngine.Repositories;
using ConfigurableWorkflowEngine.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// JSON indenting
builder.Services.Configure<JsonOptions>(opts =>
    opts.SerializerOptions.WriteIndented = true);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Configurable Workflow Engine API",
        Version = "v1"
    });
});

// In-memory repositories
builder.Services.AddSingleton<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
builder.Services.AddSingleton<IWorkflowInstanceRepository, WorkflowInstanceRepository>();

var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Honor PORT env var (for Cloud Run)
if (int.TryParse(Environment.GetEnvironmentVariable("PORT"), out var port))
    app.Urls.Add($"http://*:{port}");
else
    app.Urls.Add("http://localhost:5000");

// Map our modular endpoints
app.MapDefinitionEndpoints();
app.MapInstanceEndpoints();

// Expose Program for testing
public partial class Program { }

app.Run();
