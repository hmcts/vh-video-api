using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Responses;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.IntegrationTests.Api.Setup;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Api.InstantMessages;

public class GetInstantMessageHistoryTests : ApiTest
{
    private Conference _conference;

    [TearDown]
    public async Task TearDown()
    {
        if (_conference != null)
        {
            await TestDataManager.RemoveConference(_conference.Id);
            _conference = null;
        }
    }
    
    [Test]
    public async Task should_get_messages_for_a_conference()
    {
        // arrange
        var scheduledDateTime = DateTime.Today.AddHours(10);
        var conferenceWithMessages = new ConferenceBuilder(ignoreId: true, scheduledDateTime: scheduledDateTime)
            .WithMessages(3).Build();
        _conference = await TestDataManager.SeedConference(conferenceWithMessages);
        
        // act
        using var client = Application.CreateClient();
        var result =
            await client.GetAsync(ApiUriFactory.InstantMessageEndpoints.GetInstantMessageHistory(_conference.Id));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var messages = await Response.GetResponses<List<InstantMessageResponse>>(result.Content);
        messages.Should().NotBeNullOrEmpty();
        messages.Should().BeInDescendingOrder(x => x.TimeStamp);

        var from = conferenceWithMessages.InstantMessageHistory[0].From;
        var to = conferenceWithMessages.InstantMessageHistory[0].To;
        var messageText = conferenceWithMessages.InstantMessageHistory[0].MessageText;
        foreach (var message in messages)
        {
            message.From.Should().Be(from);
            message.MessageText.Should().Be(messageText);
            message.To.Should().Be(to);
        }
    }

    [Test]
    public async Task should_return_an_empty_list_for_a_nonexistent_conference()
    {
        // arrange
        var conferenceId = Guid.NewGuid();

        // act
        using var client = Application.CreateClient();
        var result =
            await client.GetAsync(ApiUriFactory.InstantMessageEndpoints.GetInstantMessageHistory(conferenceId));

        // assert
        result.IsSuccessStatusCode.Should().BeTrue();
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var messages = await Response.GetResponses<List<InstantMessageResponse>>(result.Content);
        messages.Should().BeEmpty();
    }
}
