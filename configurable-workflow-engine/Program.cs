using System;
using System.Linq;
using ConfigurableWorkflowEngine.Models;
using ConfigurableWorkflowEngine.Models.DTOs;
using ConfigurableWorkflowEngine.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// JSON formatting
builder.Services.Configure<JsonOptions>(opts =>
    opts.SerializerOptions.WriteIndented = true);

// register repositories
builder.Services.AddSingleton<IWorkflowDefinitionRepository, InMemoryWorkflowDefinitionRepository>();
builder.Services.AddSingleton<IWorkflowInstanceRepository, InMemoryWorkflowInstanceRepository>();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI at root: http://localhost:5000/
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workflow Engine V1");
    c.RoutePrefix = string.Empty;
});

// Create a new workflow definition
app.MapPost("/definitions", (CreateWorkflowDefinitionRequest req,
    IWorkflowDefinitionRepository defs) =>
{
    if (req.States.Select(s => s.Id).Distinct().Count() != req.States.Count)
        return Results.BadRequest("Duplicate state IDs.");
    if (req.Actions.Select(a => a.Id).Distinct().Count() != req.Actions.Count)
        return Results.BadRequest("Duplicate action IDs.");
    if (req.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("Exactly one initial state is required.");

    var stateIds = req.States.Select(s => s.Id).ToHashSet();
    foreach (var action in req.Actions)
    {
        if (!action.FromStates.All(stateIds.Contains))
            return Results.BadRequest($"Action '{action.Id}' has invalid fromStates.");
        if (!stateIds.Contains(action.ToState))
            return Results.BadRequest($"Action '{action.Id}' has invalid toState.");
    }

    var def = new WorkflowDefinition
    {
        Id = req.Id,
        Name = req.Name,
        States = req.States,
        Actions = req.Actions
    };

    try
    {
        defs.Add(def);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message);
    }

    return Results.Created($"/definitions/{def.Id}", def);
});

// Retrieve definitions
app.MapGet("/definitions/{id}", (string id, IWorkflowDefinitionRepository defs) =>
    defs.Get(id) is WorkflowDefinition d ? Results.Ok(d) : Results.NotFound());
app.MapGet("/definitions", (IWorkflowDefinitionRepository defs) => Results.Ok(defs.GetAll()));

// Start a new instance
app.MapPost("/definitions/{id}/instances", (string id,
    IWorkflowDefinitionRepository defs,
    IWorkflowInstanceRepository instances) =>
{
    var def = defs.Get(id);
    if (def == null) return Results.NotFound("Definition not found.");

    var initial = def.States.First(s => s.IsInitial);
    var inst = new WorkflowInstance
    {
        Id = Guid.NewGuid().ToString(),
        DefinitionId = def.Id,
        CurrentState = initial.Id
    };
    instances.Add(inst);
    return Results.Created($"/instances/{inst.Id}", inst);
});

// Execute an action on an instance
app.MapPost("/instances/{instanceId}/actions/{actionId}", (
    string instanceId,
    string actionId,
    IWorkflowDefinitionRepository defs,
    IWorkflowInstanceRepository instances) =>
{
    var inst = instances.Get(instanceId);
    if (inst == null) return Results.NotFound("Instance not found.");

    var def = defs.Get(inst.DefinitionId)!;
    var action = def.Actions.FirstOrDefault(a => a.Id == actionId);
    if (action == null) return Results.BadRequest("Action not part of this workflow.");
    if (!action.Enabled) return Results.BadRequest("Action is disabled.");
    if (def.States.First(s => s.Id == inst.CurrentState).IsFinal)
        return Results.BadRequest("Current state is final; no further actions allowed.");
    if (!action.FromStates.Contains(inst.CurrentState))
        return Results.BadRequest($"Action '{actionId}' cannot be applied from state '{inst.CurrentState}'.");

    var record = new TransitionRecord
    {
        ActionId = action.Id,
        FromState = inst.CurrentState,
        ToState = action.ToState,
        Timestamp = DateTime.UtcNow
    };

    inst.History.Add(record);
    inst.CurrentState = action.ToState;
    instances.Update(inst);

    return Results.Ok(inst);
});

// Retrieve instances
app.MapGet("/instances/{id}", (string id, IWorkflowInstanceRepository instances) =>
    instances.Get(id) is WorkflowInstance i ? Results.Ok(i) : Results.NotFound());
app.MapGet("/instances", (IWorkflowInstanceRepository instances) => Results.Ok(instances.GetAll()));

app.Run();
