using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Polly;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;
using VideoApi.Common;
using Microsoft.Extensions.Logging;

namespace VideoApi.Services
{
    public interface IRoomReservationService
    {
        Task<Conference> EnsureRoomAvailableAsync(Guid conferenceId, Func<Guid, Task<Conference>> getConferenceAsync);
    }

    public class RoomReservationService : IRoomReservationService
    {
        private readonly IMemoryCache _memoryCache;
        private const double CacheExpirySeconds = 10;
        private readonly ILogger<IRoomReservationService> _logger;

        public RoomReservationService(IMemoryCache memoryCache, ILogger<IRoomReservationService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
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
                        _logger.LogTrace($"PRIVATE_CONSULTATION - EnsureRoomAvailableAsync KEY : {reservationKey} : FOUND");
                        ApplicationLogger.Trace("Information", "PRIVATE_CONSULTATION", $"EnsureRoomAvailableAsync KEY : {reservationKey} : FOUND");
                        return true;
                    }

                    _logger.LogTrace($"PRIVATE_CONSULTATION - EnsureRoomAvailableAsync KEY : {reservationKey} : Not FOUND, setting cache");
                    ApplicationLogger.Trace("Information", "PRIVATE_CONSULTATION", $"EnsureRoomAvailableAsync KEY : {reservationKey} : Not FOUND, setting cache");

                    _memoryCache.Set<object>(reservationKey, null, TimeSpan.FromSeconds(CacheExpirySeconds));
                    return false;
                })
                .WaitAndRetryForeverAsync(x => TimeSpan.FromSeconds(1));
                // .WaitAndRetryAsync(10, x => TimeSpan.FromSeconds(3));

            return await retryPolicy.ExecuteAsync(async () => {
                _logger.LogTrace($"PRIVATE_CONSULTATION - Conference: {conferenceId} - EnsureRoomAvailableAsync- ExecuteAsync");
                ApplicationLogger.Trace("Information", "PRIVATE_CONSULTATION", $"Conference: {conferenceId} - EnsureRoomAvailableAsync- ExecuteAsync");
                return await getConferenceAsync(conferenceId);
            });
        }
    }
}
