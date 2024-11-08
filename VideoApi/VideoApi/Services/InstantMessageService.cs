using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services;

public interface IInstantMessageService
{
    Task AddInstantMessageAsync(Guid conferenceId, string from, string messageText, string to);
    Task RemoveInstantMessagesForConferenceAsync(Guid conferenceId);
    Task<List<InstantMessage>> GetInstantMessagesForConferenceAsync(Guid conferenceId, string participantName);
    Task<List<Conference>> GetClosedConferencesWithInstantMessages();
}

public class InstantMessageService(
    ICommandHandler commandHandler, 
    IBackgroundWorkerQueue backgroundWorkerQueue,
    IQueryHandler queryHandler)
    : IInstantMessageService
{
    public async Task AddInstantMessageAsync(Guid conferenceId, string from, string messageText, string to)
    {
        var command = new AddInstantMessageCommand(conferenceId, from, messageText, to);
        await commandHandler.Handle(command);
    }

    public async Task RemoveInstantMessagesForConferenceAsync(Guid conferenceId)
    {
        var command = new RemoveInstantMessagesForConferenceCommand(conferenceId);
        await backgroundWorkerQueue.QueueBackgroundWorkItem(command);
    }

    public async Task<List<InstantMessage>> GetInstantMessagesForConferenceAsync(Guid conferenceId, string participantName)
    {
        var query = new GetInstantMessagesForConferenceQuery(conferenceId, participantName);
        var messages =
            await queryHandler.Handle<GetInstantMessagesForConferenceQuery, List<InstantMessage>>(query);
        return messages;
    }

    public async Task<List<Conference>> GetClosedConferencesWithInstantMessages()
    {
        var query = new GetClosedConferencesWithInstantMessagesQuery();
        var closedConferences = await queryHandler.Handle<GetClosedConferencesWithInstantMessagesQuery, List<Conference>>(query);
        return closedConferences;
    }
}
