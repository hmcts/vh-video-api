using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class TaskCallResultResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var testCallResultGood = new TestCallResult(true, TestScore.Good);
            
            var testCallResultOkay = new TestCallResult(true, TestScore.Okay);

            var testCallResultBad = new TestCallResult(false, TestScore.Bad);

            var responseGood = TaskCallResultResponseMapper.MapTaskToResponse(testCallResultGood);
            var responseOkay = TaskCallResultResponseMapper.MapTaskToResponse(testCallResultOkay);
            var responseBad = TaskCallResultResponseMapper.MapTaskToResponse(testCallResultBad);

            responseGood.Should().BeEquivalentTo(testCallResultGood, options => options.Excluding(x => x.Id));
            responseOkay.Should().BeEquivalentTo(testCallResultOkay, options => options.Excluding(x => x.Id));
            responseBad.Should().BeEquivalentTo(testCallResultBad, options => options.Excluding(x => x.Id));
        }
    }
}
