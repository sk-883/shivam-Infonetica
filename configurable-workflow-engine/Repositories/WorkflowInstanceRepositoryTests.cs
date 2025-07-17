using System;
using ConfigurableWorkflowEngine.Models;
using ConfigurableWorkflowEngine.Repositories;
using Xunit;

namespace ConfigurableWorkflowEngine.Tests.Repositories
{
    public class WorkflowInstanceRepositoryTests
    {
        [Fact]
        public void Add_And_Update_Instance_Works()
        {
            var repo = new InMemoryWorkflowInstanceRepository();
            var inst = new WorkflowInstance { Id = "i1", DefinitionId = "d", CurrentState = "s" };

            repo.Add(inst);
            inst.CurrentState = "s2";
            repo.Update(inst);

            var fetched = repo.Get("i1");
            Assert.Equal("s2", fetched!.CurrentState);
        }

        [Fact]
        public void Get_Nonexistent_ReturnsNull()
        {
            var repo = new InMemoryWorkflowInstanceRepository();
            Assert.Null(repo.Get("none"));
        }
    }
}
