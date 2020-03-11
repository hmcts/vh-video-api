using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Polly;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    public interface IRoomReservationService
    {
        Task EnsureRoomAvailableAsync(Guid conferenceId, Func<Guid, Task<Conference>> getConferenceAsync);
    }

    public class RoomReservationService : IRoomReservationService
    {
        private readonly IMemoryCache _memoryCache;
        private const double CacheExpirySeconds = 5; 

        public RoomReservationService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task EnsureRoomAvailableAsync(Guid conferenceId, Func<Guid, Task<Conference>> getConferenceAsync)
        {
            var retryPolicy = Policy
                .HandleResult<bool>(reserved => reserved)
                .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(1));

            await retryPolicy.ExecuteAsync(async () =>
            {
                var conference = await getConferenceAsync(conferenceId);
                var roomType = conference.GetAvailableConsultationRoom();
                var reservationKey = $"{conferenceId}:{roomType}";

                if (_memoryCache.TryGetValue(reservationKey, out _))
                {
                    return true;
                }
                
                _memoryCache.Set<object>(reservationKey, null, TimeSpan.FromSeconds(CacheExpirySeconds));
                
                return false;
            });
        }
    }
}
