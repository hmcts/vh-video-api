using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Testing.Common.Assertions;

namespace VideoApi.UnitTests.Controllers.Conference
{
    public class ConferenceControllerTests : ConferenceControllerTestBase
    {
        [Test]
        public async Task Should_return_a_bad_request_if_an_invalid_username_was_passed_individual_username()
        {
            var username = "invalidName";
            var result = await Controller.GetConferencesTodayForIndividualByUsernameAsync(username);
            
            var typedResult = (ObjectResult)result;
            typedResult.Value.Should().NotBeNull();
            typedResult.ContainsValidationErrors();
            
            var modelState = Controller.ModelState;
            ClassicAssert.IsTrue(modelState.Keys.Contains("username"));
            ClassicAssert.AreEqual("Please provide a valid username", modelState["username"].Errors[0].ErrorMessage);
        }
    }
}
