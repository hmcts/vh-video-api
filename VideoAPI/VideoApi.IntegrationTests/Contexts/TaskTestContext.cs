using VideoApi.Contract.Requests;
using VideoApi.Domain;

namespace VideoApi.IntegrationTests.Contexts
{
    public class TaskTestContext
    {
        public Task TaskToUpdate { get; set; }
        public UpdateTaskRequest UpdateTaskRequest { get; set; }
    }
}