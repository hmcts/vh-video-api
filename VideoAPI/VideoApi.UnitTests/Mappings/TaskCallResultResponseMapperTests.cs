using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Mappings
{
    public class TaskCallResultResponseMapperTests
    {
        private readonly TaskCallResultResponseMapper _mapper = new TaskCallResultResponseMapper();
        
        [Test]
        public void should_map_all_properties()
        {
            var testCallResultGood = new TestCallResult(true, TestScore.Good);
            
            var testCallResultOkay = new TestCallResult(true, TestScore.Okay);

            var testCallResultBad = new TestCallResult(false, TestScore.Bad);

            var responseGood = _mapper.MapTaskToResponse(testCallResultGood);
            var responseOkay = _mapper.MapTaskToResponse(testCallResultOkay);
            var responseBad = _mapper.MapTaskToResponse(testCallResultBad);

            responseGood.Should().BeEquivalentTo(testCallResultGood, options => options.Excluding(x => x.Id));
            responseOkay.Should().BeEquivalentTo(testCallResultOkay, options => options.Excluding(x => x.Id));
            responseBad.Should().BeEquivalentTo(testCallResultBad, options => options.Excluding(x => x.Id));
        }
    }
}