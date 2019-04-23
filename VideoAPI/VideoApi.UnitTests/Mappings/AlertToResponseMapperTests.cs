using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Mappings
{
    public class AlertToResponseMapperTests
    {
        private readonly AlertToResponseMapper _mapper = new AlertToResponseMapper();

        [Test]
        public void should_map_all_properties()
        {
            var alert = Builder<Alert>.CreateNew().WithFactory(() => new Alert("Automated Test", AlertType.Hearing))
                .Build();
            var response = _mapper.MapAlertToResponse(alert);
            response.Should().BeEquivalentTo(alert, options => options
                .Excluding(x => x.Status)
                .Excluding(x => x.Created)
                .Excluding(x => x.Updated)
                .Excluding(x => x.UpdatedBy)
            );
        }
    }
}