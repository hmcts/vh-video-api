using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public interface ITrackable
    {
        DateTime? CreatedAt { get;  set; }
        DateTime? UpdatedAt { get;  set; }
    }

    public class TrackableEntity<TKey> : Entity<TKey>, ITrackable
    {
        private DateTime? _createdAt = DateTime.UtcNow;
        private DateTime? _updatedAt = DateTime.UtcNow;

        public DateTime? CreatedAt { get => _createdAt;  set => _createdAt = DateTime.UtcNow; }
        public DateTime? UpdatedAt { get => _updatedAt;  set => _updatedAt = DateTime.UtcNow; }
    }

}
