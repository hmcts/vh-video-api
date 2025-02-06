using System;
using System.Collections.Generic;
using System.Text;

namespace VideoApi.Services.Clients.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class RetrieveHearingResponse
    {
        /// <summary>
        /// Gets or Sets Uris
        /// </summary>
        [JsonPropertyName("uris")]
        public MeetingUris Uris { get; set; }

        /// <summary>
        /// Gets or Sets VirtualCourtroomId
        /// </summary>
        [JsonPropertyName("virtual_courtroom_id")]
        public Guid? VirtualCourtroomId { get; set; }

        /// <summary>
        /// Gets or Sets ConsultationRooms
        /// </summary>
        [JsonPropertyName("consultation_rooms")]
        public List<string> ConsultationRooms { get; set; }


        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RetrieveHearingResponse {\n");
            sb.Append("  Uris: ").Append(Uris).Append("\n");
            sb.Append("  VirtualCourtroomId: ").Append(VirtualCourtroomId).Append("\n");
            sb.Append("  ConsultationRooms: ").Append(ConsultationRooms).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
