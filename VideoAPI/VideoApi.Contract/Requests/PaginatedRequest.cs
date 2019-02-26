namespace VideoApi.Contract.Requests
{
    public class PaginatedRequest {

        public PaginatedRequest(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}