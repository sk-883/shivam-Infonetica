using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Json;
using ConfigurableWorkflowEngine.Models;
using ConfigurableWorkflowEngine.Repositories;
using ConfigurableWorkflowEngine.Dtos;

var builder = WebApplication.CreateBuilder(args);

// 1) JSON settings: camelCase
builder.Services.Configure<JsonOptions>(opts =>
    opts.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

// 2) Register repos
builder.Services.AddSingleton<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>();
builder.Services.AddSingleton<IWorkflowInstanceRepository, WorkflowInstanceRepository>();

// 3) Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 4) Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI();


// ─── Workflow Definitions ────────────────────────────────────────────────────

// Create a new workflow definition
app.MapPost("/definitions", (IWorkflowDefinitionRepository repo, [FromBody] WorkflowDefinition def) =>
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
app.MapGet("/definitions", (IWorkflowDefinitionRepository repo) =>
    Results.Ok(repo.GetAll()))
.WithName("ListDefinitions")
.WithOpenApi();

// Get definition by ID
app.MapGet("/definitions/{id}", (IWorkflowDefinitionRepository repo, string id) =>
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
app.MapPost("/instances", (IWorkflowInstanceRepository repo, [FromBody] StartInstanceRequest req) =>
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
app.MapGet("/instances", (IWorkflowInstanceRepository repo) =>
    Results.Ok(repo.GetAll()))
.WithName("ListInstances")
.WithOpenApi();

// Get an instance by ID
app.MapGet("/instances/{id}", (IWorkflowInstanceRepository repo, string id) =>
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
app.MapPost("/instances/{id}/actions/{actionId}", (IWorkflowInstanceRepository repo, string id, string actionId) =>
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
