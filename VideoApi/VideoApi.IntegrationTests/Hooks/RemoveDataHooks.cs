using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Contexts;
using static Testing.Common.Helper.ApiUriFactory.ConferenceEndpoints;

namespace VideoApi.IntegrationTests.Hooks
{
    [Binding]
    public static class RemoveDataHooks
    {
        [AfterScenario(Order = (int)HooksSequence.RemoveDataCreatedDuringTest)]
        public static async Task RemoveDataCreatedDuringTest(TestContext context)
        {
            await context.TestDataManager.RemoveConferences(context.Test.Conferences);
            await context.TestDataManager.RemoveEvents();
        }

        [BeforeScenario(Order = (int)HooksSequence.RemoveConferences)]
        [AfterScenario(Order = (int)HooksSequence.RemoveConferences)]
        public static async Task RemoveConferenceData(TestContext context)
        {
            var todaysConferences = await GetTodaysConferences(context);
            foreach (var conference in todaysConferences.Where(conference => conference.Id == context.Test.Conference.Id))
            {
                await context.TestDataManager.RemoveConference(conference.Id);
                await context.TestDataManager.RemoveHeartbeats(conference.Id);
                await context.TestDataManager.RemoveRooms(conference.Id);
            }

            await context.TestDataManager.RemoveEvents();
        }

        private static async Task<List<ConferenceDetailsResponse>> GetTodaysConferences(TestContext context)
        {
            var endpoint = GetConferencesTodayForAdmin;
            using var client = context.CreateClient();
            var response = await client.GetAsync(endpoint);
            var json = await response.Content.ReadAsStringAsync();
            return ApiRequestHelper.Deserialise<List<ConferenceDetailsResponse>>(json);
        }

        [AfterScenario(Order = (int)HooksSequence.RemoveServer)]
        public static void RemoveServer(TestContext context)
        {
            context.Server.Dispose();
        }
    }
}
