using System;

namespace VideoApi.Services.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class DoubleBookingException : Exception
    {
        public string ErrorMessage { get; }
        public DoubleBookingException(Guid conferenceId) : base(
            $"Meeting room for conference {conferenceId} has already been booked")
        {
            ErrorMessage = $"Meeting room for conference {conferenceId} has already been booked";
        }
    }
}
