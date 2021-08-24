using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class ConferenceControllerTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_a_bad_request_if_an_invalid_username_was_passed_judge_username()
        {
            var username = "invalidName";
            var result = await Controller.GetConferencesTodayForJudgeByUsernameAsync(username);

            var typedResult = (BadRequestObjectResult) result;
            typedResult.Value.Should().NotBeNull();

            var modelState = Controller.ModelState;
            Assert.IsTrue(modelState.Keys.Contains("username"));
            Assert.AreEqual("Please provide a valid username", modelState["username"].Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Should_return_a_bad_request_if_an_invalid_username_was_passed_as_host_username()
        {
            var username = "invalidName";
            var result = await Controller.GetConferencesTodayForHostAsync(username);

            var typedResult = (BadRequestObjectResult)result;
            typedResult.Value.Should().NotBeNull();

            var modelState = Controller.ModelState;
            Assert.IsTrue(modelState.Keys.Contains("username"));
            Assert.AreEqual("Please provide a valid username", modelState["username"].Errors[0].ErrorMessage);
        }

        [Test]
        public async Task Should_return_a_bad_request_if_an_invalid_username_was_passed_individual_username()
        {
            var username = "invalidName";
            var result = await Controller.GetConferencesTodayForIndividualByUsernameAsync(username);

            var typedResult = (BadRequestObjectResult)result;
            typedResult.Value.Should().NotBeNull();

            var modelState = Controller.ModelState;
            Assert.IsTrue(modelState.Keys.Contains("username"));
            Assert.AreEqual("Please provide a valid username", modelState["username"].Errors[0].ErrorMessage);
        }
    }
}
