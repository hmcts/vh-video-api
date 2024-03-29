using Microsoft.AspNetCore.Mvc;
using VideoApi.Domain;

namespace VideoApi.Validations.Models
{
    public class ConferenceParticipantExists
    {
        public Conference Conference { get; set; }
        public ParticipantBase Participant { get; set; }
        public IActionResult FailedResult { get; set; }
    }
}
