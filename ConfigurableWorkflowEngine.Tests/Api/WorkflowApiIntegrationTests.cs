using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ConfigurableWorkflowEngine.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ConfigurableWorkflowEngine.Tests.Api
{
    public class WorkflowApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WorkflowApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Definitions_CRUD_Works_EndToEnd()
        {
            // 1. GET empty list
            var defs = await _client.GetFromJsonAsync<CreateWorkflowDefinitionRequest[]>("definitions");
            Assert.Empty(defs);

            // 2. POST a new definition
            var req = new CreateWorkflowDefinitionRequest
            {
                Id = "flow1",
                Name = "Flow1",
                States = { new() { Id = "s1", Name = "Start", IsInitial = true }, new() { Id = "s2", Name = "End", IsFinal = true } },
                Actions = { new() { Id = "act", Name = "Go", FromStates = { "s1" }, ToState = "s2" } }
            };
            var createResp = await _client.PostAsJsonAsync("definitions", req);
            createResp.EnsureSuccessStatusCode();

            // 3. Verify it appears
            defs = await _client.GetFromJsonAsync<CreateWorkflowDefinitionRequest[]>("definitions");
            Assert.Single(defs);
        }
    }
}
