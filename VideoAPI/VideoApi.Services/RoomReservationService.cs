using Microsoft.Extensions.Caching.Memory;
using System;
using VideoApi.Domain;
using Microsoft.Extensions.Logging;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Services
{
    public interface IRoomReservationService
    {
        RoomType GetNextAvailableConsultationRoom(Conference conference);
        void RemoveRoomReservation(Guid conferenceId, RoomType roomType);
    }

    public class RoomReservationService : IRoomReservationService
    {
        private readonly IMemoryCache _memoryCache;
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

            // If not in cache, add to cache and return room
            if (!CheckIfRoomReserved(reservationKey, roomType))
            {
                return roomType;
            }

            // Else return next room type
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
                _memoryCache.Set(reservationKey, roomType);
            }

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
    }
}
