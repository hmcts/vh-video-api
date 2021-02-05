namespace VideoApi.DAL.Queries.Core
{
    public interface IQueryHandlerFactory
    {
        IQueryHandler<TQuery, TResult> Create<TQuery, TResult>(TQuery query) where TQuery : IQuery where TResult : class;
    }
}
