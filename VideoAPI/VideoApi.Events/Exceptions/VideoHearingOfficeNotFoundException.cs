using System;

namespace VideoApi.Events.Exceptions
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class VideoHearingOfficeNotFoundException : Exception
    {
        public VideoHearingOfficeNotFoundException(Guid hearingRefId) : base(
            $"Video Hearings Officer cannot be found for {hearingRefId}")
        {

        }
    }
}