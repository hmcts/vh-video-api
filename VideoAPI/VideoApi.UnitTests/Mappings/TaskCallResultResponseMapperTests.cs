using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.UnitTests.Mappings
{
    public class TaskCallResultResponseMapperTests
    {
        private readonly TaskCallResultResponseMapper _mapper = new TaskCallResultResponseMapper();
        
        [Test]
        public void should_map_all_properties()
        {
            var testCallResultGood = new TestCallResult
            {
                Passed = true,
                Score = TestScore.Good
            };
            
            var testCallResultOkay = new TestCallResult
            {
                Passed = true,
                Score = TestScore.Okay
            };
            
            var testCallResultBad = new TestCallResult
            {
                Passed = true,
                Score = TestScore.Bad
            };

            var responseGood = _mapper.MapTaskToResponse(testCallResultGood);
            var responseOkay = _mapper.MapTaskToResponse(testCallResultOkay);
            var responseBad = _mapper.MapTaskToResponse(testCallResultBad);

            responseGood.Should().BeEquivalentTo(testCallResultGood);
            responseOkay.Should().BeEquivalentTo(testCallResultOkay);
            responseBad.Should().BeEquivalentTo(testCallResultBad);
        }
    }
}