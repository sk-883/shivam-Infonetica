using System;
using System.Linq;
using ConfigurableWorkflowEngine.Models;
using ConfigurableWorkflowEngine.Repositories;
using Xunit;

namespace ConfigurableWorkflowEngine.Tests.Repositories
{
    public class WorkflowDefinitionRepositoryTests
    {
        [Fact]
        public void Add_And_Get_Definition_Works()
        {
            var repo = new InMemoryWorkflowDefinitionRepository();
            var def = new WorkflowDefinition { Id = "test", Name = "T", States = Array.Empty<WorkflowState>().ToList(), Actions = Array.Empty<WorkflowAction>().ToList() };

            repo.Add(def);
            var fetched = repo.Get("test");

            Assert.NotNull(fetched);
            Assert.Equal("test", fetched!.Id);
        }

        [Fact]
        public void Add_Duplicate_Throws()
        {
            var repo = new InMemoryWorkflowDefinitionRepository();
            var def = new WorkflowDefinition { Id = "dup", Name = "D", States = new(), Actions = new() };

            repo.Add(def);
            Assert.Throws<InvalidOperationException>(() => repo.Add(def));
        }
    }
}
