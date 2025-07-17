using System.Linq;
using Microsoft.AspNetCore.Builder;
using ConfigurableWorkflowEngine.Domain.Models;
using ConfigurableWorkflowEngine.DTOs;
using ConfigurableWorkflowEngine.Repositories;

namespace ConfigurableWorkflowEngine.Endpoints
{
    public static class InstanceEndpoints
    {
        public static void MapInstanceEndpoints(this WebApplication app)
        {
            // Start instance
            app.MapPost("/workflow-instances",
                (Dictionary<string, string> req,
                 IWorkflowDefinitionRepository defs,
                 IWorkflowInstanceRepository insts) =>
                {
                    if (!req.TryGetValue("definitionId", out var did)
                        || defs.Get(did) is not WorkflowDefinition def)
                    {
                        return Results.BadRequest(new ErrorResponse(
                            "Missing or invalid definitionId.",
                            "InvalidDefinitionId"));
                    }

                    var initial = def.States.Single(s => s.IsInitial);
                    var inst = new WorkflowInstance
                    {
                        DefinitionId = did,
                        CurrentStateId = initial.Id
                    };
                    insts.Add(inst);
                    return Results.Created(
                        $"/workflow-instances/{inst.Id}", inst);
                })
                .Produces<WorkflowInstance>(201)
                .Produces<ErrorResponse>(400);

            // Execute action
            app.MapPost("/workflow-instances/{instId}/actions/{actionId}",
                (string instId,
                 string actionId,
                 IWorkflowDefinitionRepository defs,
                 IWorkflowInstanceRepository insts) =>
                {
                    var inst = insts.Get(instId);
                    if (inst is null)
                        return Results.NotFound(new ErrorResponse(
                            "Instance not found.",
                            "InstanceNotFound"));

                    var def = defs.Get(inst.DefinitionId)!;
                    var action = def.Actions.SingleOrDefault(a => a.Id == actionId);
                    if (action is null || !action.Enabled)
                        return Results.BadRequest(new ErrorResponse(
                            "Invalid or disabled action.",
                            "ActionInvalid"));

                    if (!action.FromStateIds.Contains(inst.CurrentStateId))
                        return Results.BadRequest(new ErrorResponse(
                            "Action not allowed from current state.",
                            "ActionNotAllowed"));

                    var current = def.States.Single(s => s.Id == inst.CurrentStateId);
                    if (current.IsFinal)
                        return Results.BadRequest(new ErrorResponse(
                            "Cannot act on final state.",
                            "InstanceInFinalState"));

                    var target = def.States.Single(s => s.Id == action.ToStateId);
                    if (!target.Enabled)
                        return Results.BadRequest(new ErrorResponse(
                            "Target state disabled.",
                            "TargetStateDisabled"));

                    // transition
                    inst.CurrentStateId = target.Id;
                    inst.History.Add(new HistoryRecord(actionId));
                    return Results.Ok(inst);
                })
                .Produces<WorkflowInstance>()
                .Produces<ErrorResponse>(400)
                .Produces<ErrorResponse>(404);

            // Get instance
            app.MapGet("/workflow-instances/{instId}",
                (string instId, IWorkflowInstanceRepository insts) =>
                {
                    var inst = insts.Get(instId);
                    return inst is not null
                        ? Results.Ok(inst)
                        : Results.NotFound(new ErrorResponse(
                            "Instance not found.",
                            "InstanceNotFound"));
                })
                .Produces<WorkflowInstance>()
                .Produces<ErrorResponse>(404);

            // List instances (filter + page)
            app.MapGet("/workflow-instances",
                (string? definitionId,
                 string? currentStateId,
                 int? pageNumber,
                 int? pageSize,
                 IWorkflowInstanceRepository insts) =>
                {
                    var query = insts.List();
                    if (!string.IsNullOrEmpty(definitionId))
                        query = query.Where(i => i.DefinitionId == definitionId);
                    if (!string.IsNullOrEmpty(currentStateId))
                        query = query.Where(i => i.CurrentStateId == currentStateId);

                    var all = query.ToList();
                    int pn = pageNumber.GetValueOrDefault(1);
                    int ps = pageSize.GetValueOrDefault(10);
                    var items = all.Skip((pn - 1) * ps).Take(ps);
                    return Results.Ok(new PagedResponse<WorkflowInstance>(
                        items, all.Count, pn, ps));
                })
                .Produces<PagedResponse<WorkflowInstance>>();
        }
    }
}
