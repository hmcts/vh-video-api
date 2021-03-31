using Faker;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain.Enums;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetDistinctJudgeListByFirstNameQueryTests : DatabaseTestsBase
    {
        private GetDistinctJudgeListByFirstNameQueryHandler _handler;
        private IList<Guid> _conferenceIds;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetDistinctJudgeListByFirstNameQueryHandler(context);
            _conferenceIds = new List<Guid>();
        }

        [Test]
        public async Task Should_get_a_list_of_judges()
        {
            for (int i = 0; i < 3; i++)
            {
                var newConference = new ConferenceBuilder(true)
               .WithParticipant(UserRole.Representative, "Respondent")
               .WithParticipant(UserRole.Judge, null)
               .Build();
                _conferenceIds.Add(newConference.Id);

                await TestDataManager.SeedConference(newConference);
            }

            var judgelist = await _handler.Handle(new GetDistinctJudgeListByFirstNameQuery());

            judgelist.Should().NotBeEmpty();
        }

        [Test]
        public async Task Should_get_unique_list_of_judges()
        {
            string judge1 = CreateUniqueName();
            for (int i = 0; i < 2; i++)
            {
                await CreateConference(judge1);
            }

            string judge2 = CreateUniqueName();
            for (int i = 0; i < 3; i++)
            {
                await CreateConference(judge2);
            }

            var judgelist = await _handler.Handle(new GetDistinctJudgeListByFirstNameQuery());

            judgelist.Count(x => x.Contains(judge1)).Should().Be(1);
            judgelist.Count(x => x.Contains(judge2)).Should().Be(1);
            judgelist.Count.Should().BeGreaterOrEqualTo(2);
            judgelist.Should().NotBeEmpty();
        }

        [Test]
        public async Task Should_get_list_of_judges_excluding_anonymised_and_automation_test_users()
        {
            var newConference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Judge, "Judge", "Judge.James@hmcts.net", "JudgeManchester", null, ParticipantState.None)
                .WithParticipant(UserRole.Representative, "Applicant", "Applicant.Smith@hmcts.net", "Applicant", null, ParticipantState.None)
                .WithParticipant(UserRole.Individual, "Applicant", "ApplicantLIP.Green@hmcts.net", "Applicant", null, ParticipantState.None)
                .Build();
            _conferenceIds.Add(newConference.Id);
            await TestDataManager.SeedConference(newConference);

            // anonymised data
            newConference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Judge, "Judge", "Judge.James1@hmcts.net1", "JudgeLondon", null, ParticipantState.None)
                .WithParticipant(UserRole.Representative, "Applicant", "Applicant.Smith1@hmcts.net", "Applicant", null, ParticipantState.None)
                .WithParticipant(UserRole.Individual, "Applicant", "ApplicantLIP.Green1@hmcts.net", "Applicant", null, ParticipantState.None)
                .Build();
            _conferenceIds.Add(newConference.Id);
            await TestDataManager.SeedConference(newConference);

            var judgeList = await _handler.Handle(new GetDistinctJudgeListByFirstNameQuery());
            judgeList.Should().NotBeEmpty();
            judgeList.Should().NotContain("JudgeLondon");
        }

        private async Task CreateConference(string judge)
        {
            var newConference = new ConferenceBuilder(true)
              .WithParticipant(UserRole.Representative, "Respondent")
              .WithParticipant(UserRole.Judge, null, firstName: judge)
              .Build();
            _conferenceIds.Add(newConference.Id);

            await TestDataManager.SeedConference(newConference);
        }
        

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for GetDistinctJudgeListByFirstNameQueryTests");
            foreach (var conferenceId in _conferenceIds)
            {
                await TestDataManager.RemoveConference(conferenceId);
            }
        }
        
        private string CreateUniqueName()
        {
            //This is done to create an unique first name to avoid the test from failing because of any existing data
            return $"Automation_{Name.First()}{Faker.RandomNumber.Next()}";
        }
    }
}
