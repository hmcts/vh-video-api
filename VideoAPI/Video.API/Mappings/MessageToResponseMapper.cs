using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class MessageToResponseMapper
    {
        public MessageResponse MapMessageToResponse(Message message)
        {
            return new MessageResponse
            {
                Id = message.Id,
                From = message.From,
                To = message.To,
                MessageText = message.MessageText,
                TimeStamp = message.TimeStamp
            };
        }
    }
}
