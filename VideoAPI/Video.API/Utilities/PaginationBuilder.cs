using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Common;
using VideoApi.Contract.Responses;

namespace Video.API.Utilities
{
    /// <summary>
    /// Builder to add pagination to a queryable list of domain objects
    /// </summary>
    /// <typeparam name="TResponseType">The final response object to create</typeparam>
    /// <typeparam name="TModelType">The model that will be paged</typeparam>
    public class PaginationBuilder<TResponseType, TModelType> where TResponseType : PagedResponse
    {
        private readonly Func<List<TModelType>, TResponseType> _factory;
        private string _resourceUrl;
        private IQueryable<TModelType> _items;
        private int _pageSize = 5;
        private int _page = 1;
        private int _totalItems;

        public PaginationBuilder(Func<List<TModelType>, TResponseType> factory)
        {
            _factory = factory;
        }

        public PaginationBuilder<TResponseType,TModelType> ResourceUrl(string resourceUrl)
        {
            _resourceUrl = resourceUrl;
            return this;
        }

        public PaginationBuilder<TResponseType, TModelType> WithSourceItems(IQueryable<TModelType> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _totalItems = items.Count();
            return this;
        }

        public PaginationBuilder<TResponseType, TModelType> PageSize(int pageSize)
        {
            if (pageSize < 1)
                throw new ArgumentException("Page size needs to greater or equal to one", nameof(pageSize));

            _pageSize = pageSize;
            return this;
        }

        public PaginationBuilder<TResponseType, TModelType> CurrentPage(int page)
        {
            if (page < 1)
                throw new ArgumentException("Page must be greater or equal to one", nameof(page));

            _page = page;
            return this;
        }

        public TResponseType Build()
        {
            if (string.IsNullOrEmpty(_resourceUrl))
                throw new InvalidOperationException($"Resource url must be defined before using {nameof(Build)}");

            if (_page > TotalPages)
                throw new BadRequestException($"Cannot access page [{_page}] of total [{TotalPages}] pages");

            var pagedItems = _items.Skip(_pageSize * (_page - 1)).Take(_pageSize).ToList();
            var response = _factory(pagedItems);

            response.CurrentPage = _page;
            response.TotalCount = _totalItems;
            response.TotalPages = TotalPages;
            response.PageSize = _pageSize;
            response.NextPageUrl = GetPageUrl(NextPage);
            response.PrevPageUrl = GetPageUrl(PrevPage);

            return response;
        }

        private string GetPageUrl(int? page)
        {
            return !page.HasValue ? null : $"{_resourceUrl}?page={page.Value}&pageSize={_pageSize}";
        }

        private int TotalPages => _totalItems > 0 ? (int) Math.Ceiling((double) _totalItems / _pageSize) : 1;

        private int? NextPage => _page < TotalPages ? _page + 1 : new int?();

        private int? PrevPage => _page > 1 ? _page - 1 : new int?();
    }
}