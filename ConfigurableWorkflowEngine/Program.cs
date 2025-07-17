
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Json;
using WorkflowEngine.Models;
using WorkflowEngine.Repositories;

public record ErrorResponse(string Error, string Code, string Details, DateTime Timestamp);
public record StartInstanceRequest(string DefinitionId);

var builder = WebApplication.CreateBuilder(args);

// 1) JSON settings (camelCase)
builder.Services.Configure<JsonOptions>(opts =>
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// 2) Register repositories
builder.Services.AddSingleton<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
builder.Services.AddSingleton<IWorkflowInstanceRepository, WorkflowInstanceRepository>();

// 3) Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4) Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

// ─── Workflow Definitions ────────────────────────────────────────────────────

// Create a new workflow definition
app.MapPost("/definitions", ([FromServices] IWorkflowDefinitionRepository repo, [FromBody] WorkflowDefinition def) =>
{
    try
    {
        repo.Create(def);
        return Results.Created($"/definitions/{def.Id}", def);
    }
    catch (InvalidOperationException ex)
    {
        var err = new ErrorResponse(
            ex.Message,
            ex.GetType().Name,
            "Please verify your states/actions and try again.",
            DateTime.UtcNow
        );
        return Results.BadRequest(err);
    }
})
.WithName("CreateDefinition")
.WithOpenApi();

// List all definitions
app.MapGet("/definitions", ([FromServices] IWorkflowDefinitionRepository repo) =>
    Results.Ok(repo.GetAll()))
.WithName("ListDefinitions")
.WithOpenApi();

// Get a single definition
app.MapGet("/definitions/{id}", ([FromServices] IWorkflowDefinitionRepository repo, string id) =>
{
    var def = repo.Get(id);
    return def is not null
        ? Results.Ok(def)
        : Results.NotFound(new ErrorResponse(
            $"Definition '{id}' not found.",
            "NotFound",
            "Ensure the workflow definition ID is correct.",
            DateTime.UtcNow
        ));
})
.WithName("GetDefinition")
.WithOpenApi();

// ─── Workflow Instances ─────────────────────────────────────────────────────

// Start a new instance
app.MapPost("/instances", ([FromServices] IWorkflowInstanceRepository repo, [FromBody] StartInstanceRequest req) =>
{
    try
    {
        var inst = repo.Create(req.DefinitionId);
        return Results.Created($"/instances/{inst.Id}", inst);
    }
    catch (InvalidOperationException ex)
    {
        var err = new ErrorResponse(
            ex.Message,
            ex.GetType().Name,
            $"Cannot start instance for definition '{req.DefinitionId}'.",
            DateTime.UtcNow
        );
        return Results.BadRequest(err);
    }
})
.WithName("StartInstance")
.WithOpenApi();

// List all instances
app.MapGet("/instances", ([FromServices] IWorkflowInstanceRepository repo) =>
    Results.Ok(repo.GetAll()))
.WithName("ListInstances")
.WithOpenApi();

// Get one instance
app.MapGet("/instances/{id}", ([FromServices] IWorkflowInstanceRepository repo, string id) =>
{
    var inst = repo.Get(id);
    return inst is not null
        ? Results.Ok(inst)
        : Results.NotFound(new ErrorResponse(
            $"Instance '{id}' not found.",
            "NotFound",
            "Verify the workflow instance ID.",
            DateTime.UtcNow
        ));
})
.WithName("GetInstance")
.WithOpenApi();

// Execute an action on an instance
app.MapPost("/instances/{id}/actions/{actionId}", ([FromServices] IWorkflowInstanceRepository repo, string id, string actionId) =>
{
    try
    {
        repo.ExecuteAction(id, actionId);
        return Results.Ok(repo.Get(id));
    }
    catch (InvalidOperationException ex)
    {
        var err = new ErrorResponse(
            ex.Message,
            ex.GetType().Name,
            $"Failed to fire action '{actionId}' on instance '{id}'.",
            DateTime.UtcNow
        );
        return Results.BadRequest(err);
    }
})
.WithName("ExecuteAction")
.WithOpenApi();

app.Run();
