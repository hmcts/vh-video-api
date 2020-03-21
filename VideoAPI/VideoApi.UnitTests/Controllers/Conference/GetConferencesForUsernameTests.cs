using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Testing.Common.Assertions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers
{
    public class GetConferencesForUsernameTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_ok_result_for_given_valid_user_name()
        {
            var result = await Controller.GetConferencesForUsernameAsync("test@tester.com");

            var typedResult = (OkObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task Should_return_bad_request_for_given_invalid_user_name()
        {
            var username = string.Empty;

            var result = await Controller.GetConferencesForUsernameAsync(username);

            var typedResult = (ObjectResult)result;
            typedResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            ((SerializableError)typedResult.Value).ContainsKeyAndErrorMessage(nameof(username), $"Please provide a valid {nameof(username)}");
        }

    }
}
