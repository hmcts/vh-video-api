
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class GetDistinctJudgeNamesTests : ParticipantsControllerTestBase
    {

        [SetUp]
        public void TestInitialize()
        {
            var judgeFirstnames = new List<string> { "JudgeName1", "JudgeName2", "JudgeName3" };

            MockQueryHandler
                .Setup(x => x.Handle<GetDistinctJudgeListByFirstNameQuery, List<string>>(It.IsAny<GetDistinctJudgeListByFirstNameQuery>()))
                .ReturnsAsync(judgeFirstnames);
        }

        [Test]
        public async Task Should_get_judge_list_response()
        {
            var result = await Controller.GetDistinctJudgeNamesAsync();

            MockQueryHandler.Verify(m => m.Handle<GetDistinctJudgeListByFirstNameQuery, List<string>>(It.IsAny<GetDistinctJudgeListByFirstNameQuery>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            var response = result.As<OkObjectResult>().Value.As<JudgeNameListResponse>();
            response.FirstNames.Count.Should().Be(3);
        }
    }
}

