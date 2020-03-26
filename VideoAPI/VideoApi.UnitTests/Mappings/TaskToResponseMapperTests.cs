using System;
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
        [Test]
        public void Should_map_all_properties()
        {
            var alert = Builder<Task>.CreateNew()
                .WithFactory(() => new Task(Guid.NewGuid(), "Automated Test", TaskType.Hearing)).Build();
            var response = TaskToResponseMapper.MapTaskToResponse(alert);
            response.Should().BeEquivalentTo(alert, options => options
                .Excluding(x => x.Status)
                .Excluding(x => x.Created)
                .Excluding(x => x.Updated)
                .Excluding(x => x.UpdatedBy)
            );
        }
    }
}
