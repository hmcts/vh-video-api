using System;

namespace VideoApi.Domain
{
    public static class MagicLink
    {
        public static bool IsValid(Conference conference)
        {
            if (conference == null || conference.IsClosed() || IsConferenceStartYesterdayOrEarlier(conference))
                return false;

            return true;
        }

        private static bool IsConferenceStartYesterdayOrEarlier(Conference conference)
        {

            return conference.ScheduledDateTime < DateTime.Today;
        }
    }
}
