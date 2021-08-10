using NUnit.Framework;
using System;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.UnitTests.Domain
{
    public class QuickLinkTests
    {
        private VideoApi.Domain.Conference _conference;

        [SetUp]
        public void SetUp()
        {
            _conference = new ConferenceBuilder()
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .Build();
        }

        [Test]
        public void Should_return_false_if_conference_is_closed()
        {
            //Arrange
            _conference.UpdateConferenceStatus(ConferenceState.Closed);

            ///Act
            var result = QuickLink.IsValid(_conference);

            //Assert
            Assert.False(result);
        }

        [Test]
        public void Should_return_false_if_conference_is_yesterday()
        {
            //Arrange
            _conference = new ConferenceBuilder(scheduledDateTime: DateTime.Today.AddDays(-1))
                .WithParticipant(UserRole.Judge, null)
                .WithParticipant(UserRole.Individual, "Applicant", null, null, RoomType.ConsultationRoom)
                .WithParticipant(UserRole.Representative, "Applicant")
                .Build();

            ///Act
            var result = QuickLink.IsValid(_conference);

            //Assert
            Assert.False(result);
        }

        [Test]
        public void Should_return_false_if_conference_is_null()
        {
            //Arrange/Act
            var result = QuickLink.IsValid(null);

            //Assert
            Assert.False(result);
        }

        [Test]
        public void Should_return_true()
        {
            //Arrange/Act
            var result = QuickLink.IsValid(_conference);

            //Assert
            Assert.True(result);
        }
    }
}
