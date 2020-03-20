using System;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;

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

        private static void RemoveConference(TestContext context, Guid conferenceId)
        {
            context.Request = context.Delete(ApiUriFactory.ConferenceEndpoints.RemoveConference(conferenceId));
            context.Response = context.Client().Execute(context.Request);
            context.Response.IsSuccessful.Should().BeTrue("Conference is deleted");
        }
    }
}
