namespace VideoApi.Contract.Responses
{
    /// <summary>
    /// Base class for paged responses where each response represents a subset of the entire data amount to be listed
    /// </summary>
    public abstract class PagedResponse
    {
        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Maximum number of items returned in items
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of pages given the current page size
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Numbering of this paged response, starting from 1
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Absolute url to the previous page of items.
        /// Will be null for the first page.
        /// </summary>
        public string PrevPageUrl { get; set; }

        /// <summary>
        /// Absolute url for the next page of items.
        /// Will be null for the last page.
        /// </summary>
        public string NextPageUrl { get; set; }
    }
}