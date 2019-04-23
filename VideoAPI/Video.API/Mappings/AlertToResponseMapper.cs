using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public class AlertToResponseMapper
    {
        public AlertResponse MapAlertToResponse(Alert alert)
        {
            return new AlertResponse()
            {
                Id = alert.Id,
                Body = alert.Body,
                Type = alert.Type
            };
        }
    }
}