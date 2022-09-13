using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Extentions
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IQueryable<T> source, PagingParameter pagingParam)
        {
            var count = source.Count();
            var items = source.Skip((pagingParam.PageNumber - 1) * pagingParam.PageSize).Take(pagingParam.PageSize).ToList();
            return new PagedList<T>(items, count, pagingParam.PageNumber, pagingParam.PageSize);
        }

        public static void PrepareHTTPResponseMetadata(HttpResponse response, PagedList<T> result)
        {
            var metadata = new
            {
                result.CurrentPage,
                result.HasNext,
                result.HasPrevious,
                result.TotalPages,
                result.TotalCount
            };

            response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
        }
    }
}
