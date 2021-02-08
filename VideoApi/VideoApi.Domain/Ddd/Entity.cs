namespace VideoApi.Domain.Ddd
{
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public virtual TKey Id { get; protected set; }
    }
}