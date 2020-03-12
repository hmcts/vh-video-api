using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class InstantMessageToResponseMapper
    {
        public InstantMessageResponse MapMessageToResponse(InstantMessage instantMessage)
        {
            return new InstantMessageResponse
            {
                From = instantMessage.From,
                MessageText = instantMessage.MessageText,
                TimeStamp = instantMessage.TimeStamp
            };
        }
    }
}
