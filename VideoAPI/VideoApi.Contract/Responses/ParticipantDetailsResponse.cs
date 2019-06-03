using System;
using System.Collections.Generic;
using VideoApi.Domain.Enums;

namespace VideoApi.Contract.Responses
{
    public class ParticipantDetailsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public UserRole UserRole { get; set; }
        public string CaseTypeGroup { get; set; }
        public string Representee { get; set; }
        public ParticipantStatusResponse CurrentStatus { get; set; }
        public TestCallScoreResponse SelfTestScore { get; set; }
    }
}