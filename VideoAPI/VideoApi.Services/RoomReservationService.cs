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
        Task<Conference> EnsureRoomAvailableAsync(Guid conferenceId, Func<Guid, Task<Conference>> getConferenceAsync);
    }

    public class RoomReservationService : IRoomReservationService
    {
        private readonly IMemoryCache _memoryCache;
        private const double CacheExpirySeconds = 5; 

        public RoomReservationService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<Conference> EnsureRoomAvailableAsync(Guid conferenceId, Func<Guid, Task<Conference>> getConferenceAsync)
        {
            var retryPolicy = Policy
                .HandleResult<Conference>(x =>
                {
                    var roomType = x.GetAvailableConsultationRoom();
                    var reservationKey = $"{conferenceId}:{roomType}";
                    if (_memoryCache.TryGetValue(reservationKey, out _))
                    {
                        return true;
                    }
                    _memoryCache.Set<object>(reservationKey, null, TimeSpan.FromSeconds(CacheExpirySeconds));
                    return false;
                })
                .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(1));
            return await retryPolicy.ExecuteAsync(async () => await getConferenceAsync(conferenceId));
        }
    }
}
