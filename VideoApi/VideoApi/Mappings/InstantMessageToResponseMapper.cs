using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class InstantMessageToResponseMapper
    {
        public static InstantMessageResponse MapMessageToResponse(InstantMessage instantMessage)
        {
            return new InstantMessageResponse
            {
                From = instantMessage.From,
                MessageText = instantMessage.MessageText,
                TimeStamp = instantMessage.TimeStamp,
                To = instantMessage.To
            };
        }
    }
}
