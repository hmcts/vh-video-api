using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddAudioRecordingAlertCommand : ICommand
    {
        public Guid ConferenceId { get; }

        public AddAudioRecordingAlertCommand(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }
    }

    public class AddAudioRecordingAlertCommandHandler : ICommandHandler<AddAudioRecordingAlertCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddAudioRecordingAlertCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddAudioRecordingAlertCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }
            
            var caseNumber = conference.CaseNumber;
            
            var record = new AudioRecordingAlert(command.ConferenceId, caseNumber);
            
            _context.AudioRecordingAlerts.Add(record);
            
            _context.Entry(record).State = EntityState.Added;
            await _context.SaveChangesAsync();
        }
    }
}
