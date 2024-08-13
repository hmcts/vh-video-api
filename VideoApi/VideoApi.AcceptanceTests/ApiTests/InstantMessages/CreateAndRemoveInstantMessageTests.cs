using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace VideoApi.AcceptanceTests.ApiTests.InstantMessages
{

    public class CreateAndRemoveInstantMessageTests : AcApiTest
    {
        private ConferenceDetailsResponse _conference;
        private string _fromUsername;
        private string _toUsername;
        private string _messageText = "AC Test IMs";

        [Test]
        public async Task should_add_get_and_remove_instant_messages_for_a_hearing()
        {
            await CreateAConference();
            AddInstantMessagesToConference();
            await GetInstantMessagesForConference();
            RemoveInstantMessagesForConference();
        }

        [Test]
        public async Task should_get_message_history_for_a_participant()
        {
            await CreateAConference();
            AddInstantMessagesToConference();
            await GetInstantMessagesForParticipant();
            RemoveInstantMessagesForConference();
        }

        [Test]
        public async Task should_get_an_empty_message_history_for_a_nonexistent_participant()
        {
            var history = await VideoApiClient.GetInstantMessageHistoryForParticipantAsync(Guid.NewGuid(), "not.exist.com");
            history.Should().BeEmpty();
        }
        
        [TearDown]
        public async Task TearDown()
        {
            _fromUsername = null;
            _toUsername = null;
            if (_conference != null)
            {
                await VideoApiClient.RemoveConferenceAsync(_conference.Id);
                _conference = null;
            }
        }

        private async Task CreateAConference()
        {
            var date = DateTime.Now.ToLocalTime().AddMinutes(2);
            var sipStem = GetSupplierSipAddressStem();
            var request = new BookNewConferenceRequestBuilder("AC InstantMessage Tests", sipStem)
                .WithJudge()
                .WithRepresentative().WithIndividual()
                .WithHearingRefId(Guid.NewGuid())
                .WithDate(date)
                .Build();
            _conference = await VideoApiClient.BookNewConferenceAsync(request);
        }

        private void AddInstantMessagesToConference()
        {
            _fromUsername = _conference.Participants.Find(x => x.UserRole == UserRole.Judge).Username;
            _toUsername = _conference.Participants.Find(x => x.UserRole == UserRole.Individual).Username;
            var request = new AddInstantMessageRequest()
            {
                From = _fromUsername,
                To = _toUsername,
                MessageText = _messageText
            };

            Assert.DoesNotThrowAsync(async () => await VideoApiClient.AddInstantMessageToConferenceAsync(_conference.Id, request));
        }

        private async Task GetInstantMessagesForConference()
        {
            var messages = await VideoApiClient.GetInstantMessageHistoryAsync(_conference.Id);
            messages.Should().NotBeNullOrEmpty();
            var message = messages.First();
            AssertMessage(message);
        }
        
        private async Task GetInstantMessagesForParticipant()
        {
            var messages = await VideoApiClient.GetInstantMessageHistoryForParticipantAsync(_conference.Id, _toUsername);
            messages.Should().NotBeNullOrEmpty();
            var message = messages.First();
            AssertMessage(message);
        }

        private void AssertMessage(InstantMessageResponse message)
        {
            message.From.Should().Be(_fromUsername);
            message.MessageText.Should().Be(_messageText);
            message.To.Should().Be(_toUsername);
        }

        private void RemoveInstantMessagesForConference()
        {
            Assert.DoesNotThrowAsync(async () => await VideoApiClient.RemoveInstantMessagesAsync(_conference.Id));
        }
    }
}
