using System;
using System.Collections.Generic;
using System.Linq;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Responses;
using static Testing.Common.Helper.ApiUriFactory.ConferenceEndpoints;

namespace VideoApi.AcceptanceTests.Hooks
{
    [Binding]
    public static class RemoveDataHooks
    {
        [AfterScenario(Order = (int)HooksSequence.RemoveConference)]
        public static void RemoveConference(TestContext context)
        {
            if (context.Test.Conference == null) return;
            RemoveConference(context, context.Test.Conference.Id);
        }

        [AfterScenario(Order = (int)HooksSequence.RemoveConferences)]
        public static void RemoveConferences(TestContext context)
        {
            if (context.Test.ConferenceIds.Count <= 0) return;
            foreach (var id in context.Test.ConferenceIds)
            {
                RemoveConference(context, id);
            }
        }

        [AfterScenario(Order = (int)HooksSequence.RemoveAllTodaysConferences)]
        public static void RemoveAllTodaysConferences(TestContext context)
        {
            context.Request = context.Get(GetConferencesTodayForAdmin);
            context.Response = context.Client().Execute(context.Request);
            var conferences = RequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ConferenceForAdminResponse>>(context.Response.Content);
            if (conferences == null || conferences.Count <= 0) return;
            foreach (var conference in conferences.Where(conference => conference.CaseName.Contains(context.Test.CaseName)))
            {
                RemoveConference(context, conference.Id);
            }
        }

        private static void RemoveConference(TestContext context, Guid conferenceId)
        {
            context.Request = context.Delete(ApiUriFactory.ConferenceEndpoints.RemoveConference(conferenceId));
            context.Response = context.Client().Execute(context.Request);
            context.Response.IsSuccessful.Should().BeTrue("Conference is deleted");
        }
    }
}
