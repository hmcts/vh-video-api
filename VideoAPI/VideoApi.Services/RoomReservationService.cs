using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using Polly;
using VideoApi.Domain;
using VideoApi.Common;
using Microsoft.Extensions.Logging;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Services
{
    public interface IRoomReservationService
    {
        Task<Conference> EnsureRoomAvailableAsync(Guid conferenceId, string requestedBy, string requestedFor, Func<Guid, Task<Conference>> getConferenceAsync);
    }

    public class RoomReservationService : IRoomReservationService
    {
        private readonly IMemoryCache _memoryCache;
        private const double CacheExpirySeconds = 30;
        private readonly ILogger<IRoomReservationService> _logger;

        public RoomReservationService(IMemoryCache memoryCache, ILogger<IRoomReservationService> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }
        
        public RoomType GetNextAvailableConsultationRoom(Conference conference)
        {
            var roomType = conference.GetAvailableConsultationRoom();
            var reservationKey = $"{conference.Id}:{roomType}";

            // if not in cache, add to cache and return room
            if(!CheckIfRoomReserved(reservationKey, roomType))
            {
                return roomType;
            }
            

            // else return next room type
            if (roomType == RoomType.ConsultationRoom1)
            {
                roomType = RoomType.ConsultationRoom2; 
                reservationKey = $"{conference.Id}:{roomType}";
                if (!CheckIfRoomReserved(reservationKey, roomType))
                {
                    return roomType;
                }
            }
            throw new DomainRuleException("Unavailable room", "No consultation rooms available");
        }

        private bool CheckIfRoomReserved(string reservationKey, RoomType roomType)
        {
            var isReserved = _memoryCache.TryGetValue(reservationKey, out _);
            if (!isReserved)
            {
                
            }
            _memoryCache.Set(reservationKey, roomType);
            return isReserved;
        }

        public void RemoveRoomReservation(Guid conferenceId, RoomType roomType)
        {
            var reservationKey = $"{conferenceId}:{roomType}";
            if (_memoryCache.TryGetValue(reservationKey, out _))
            {
                _memoryCache.Remove(reservationKey);
            }
        }

        public async Task<Conference> EnsureRoomAvailableAsync(Guid conferenceId, string requestedBy, string requestedFor, Func<Guid, Task<Conference>> getConferenceAsync)
        {
            var retryPolicy = Policy
                .HandleResult<Conference>(x =>
                {
                    var roomType = x.GetAvailableConsultationRoom();
                    var reservationKey = $"{conferenceId}:{roomType}";

                    if (_memoryCache.TryGetValue(reservationKey, out _))
                    {
                        ApplicationLogger.Trace("PRIVATE_CONSULTATION", "EnsureRoomAvailableAsync", $"EnsureRoomAvailableAsync - Between {requestedBy} and {requestedFor} - KEY : {reservationKey} : FOUND");
                        return true;
                    }

                    ApplicationLogger.Trace("PRIVATE_CONSULTATION", "EnsureRoomAvailableAsync", $"EnsureRoomAvailableAsync- Between {requestedBy} and {requestedFor} - KEY : {reservationKey} : Not FOUND, setting cache");

                    _memoryCache.Set<object>(reservationKey, null, TimeSpan.FromSeconds(CacheExpirySeconds));
                    return false;
                })
                //.WaitAndRetryForeverAsync(x => TimeSpan.FromSeconds(1));
                .WaitAndRetryAsync(10, x => TimeSpan.FromSeconds(3));

            return await retryPolicy.ExecuteAsync(async () => {
                ApplicationLogger.Trace("PRIVATE_CONSULTATION", "EnsureRoomAvailableAsync", $"EnsureRoomAvailableAsync- ExecuteAsync - Between {requestedBy} and {requestedFor} - Conference: {conferenceId}");
                return await getConferenceAsync(conferenceId);
            });
        }
    }
}
