using System;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class AudioRecordingAlert : TrackableEntity<Guid>
    {
        public Guid ConferenceId { get; set; }
        public string CaseNumber { get; private set; }
        
        public DateTime? CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private AudioRecordingAlert()
        {
            Id = Guid.NewGuid();
        }

        public AudioRecordingAlert(Guid conferenceId, string caseNumber) : this()
        {
            ConferenceId = conferenceId;
            CaseNumber = caseNumber;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
        }
        
    }
}
