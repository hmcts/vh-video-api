using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using VideoApi.Domain.Enums;

namespace VideoApi.Services
{
    public interface IConsultationCache
    {
        Task AddConsultationRoomToCache(Guid conferenceId, RoomType room);

        Task<RoomType?> GetConsultationRoom(Guid conferenceId);

        void Remove(Guid conferenceId);
    }

    public class ConsultationCache : IConsultationCache
    {
        private readonly IMemoryCache _memoryCache;

        // Cache expiry in seconds
        private const double CACHE_EXPIRY = 10;

        public ConsultationCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task AddConsultationRoomToCache(Guid conferenceId, RoomType room)
        {
            await _memoryCache.GetOrCreateAsync(conferenceId, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromSeconds(CACHE_EXPIRY));
                return Task.FromResult(room);
            });
        }

        public Task<RoomType?> GetConsultationRoom(Guid conferenceId)
        {
            RoomType? room;
            _memoryCache.TryGetValue(conferenceId, out room);
            return Task.FromResult(room);
        }

        public void Remove(Guid conferenceId)
        {
            _memoryCache.Remove(conferenceId);
        }
    }
}
