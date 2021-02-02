using System;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class TaskToResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var alert = Builder<Task>.CreateNew()
                .WithFactory(() => new Task(Guid.NewGuid(), Guid.NewGuid(), "Automated Test", TaskType.Hearing))
                .Build();
            var response = TaskToResponseMapper.MapTaskToResponse(alert);
            response.Should().BeEquivalentTo(alert, options => options
                .Excluding(x => x.Status)
                .Excluding(x => x.Created)
                .Excluding(x => x.Updated)
                .Excluding(x => x.UpdatedBy)
                .Excluding(x => x.ConferenceId)
            );
        }
    }
}
