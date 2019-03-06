using System;
using System.Linq;
using Faker;
using FizzWare.NBuilder;
using VideoApi.Contract.Requests;

namespace Testing.Common.Helper.Builders.Api
{
    public class BookNewConferenceRequestBuilder
    {
        private readonly BookNewConferenceRequest _bookNewConferenceRequest;
        
        public BookNewConferenceRequestBuilder()
        {
            var participants = Builder<ParticipantRequest>.CreateListOfSize(3)
                .All()
                .With(x => x.Name = Name.FullName())
                .With(x => x.Username = Internet.Email())
                .With(x => x.DisplayName = Internet.UserName())
                .Build().ToList();
            
            _bookNewConferenceRequest = Builder<BookNewConferenceRequest>.CreateNew()
                .With(x => x.ScheduledDateTime = DateTime.Today.AddDays(5).AddHours(10).AddMinutes(30))
                .With(x => x.Participants = participants)
                .Build();
        }
        
        public BookNewConferenceRequest Build()
        {
            return _bookNewConferenceRequest;
        }
    }
}