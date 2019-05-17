using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Mappings
{
    public class TaskToResponseMapperTests
    {
        private readonly TaskToResponseMapper _mapper = new TaskToResponseMapper();

        [Test]
        public void should_map_all_properties()
        {
            var alert = Builder<Task>.CreateNew().WithFactory(() => new Task("Automated Test", TaskType.Hearing))
                .Build();
            var response = _mapper.MapTaskToResponse(alert);
            response.Should().BeEquivalentTo(alert, options => options
                .Excluding(x => x.Status)
                .Excluding(x => x.Created)
                .Excluding(x => x.Updated)
                .Excluding(x => x.UpdatedBy)
            );
        }
    }
}