using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Taxi.Models;

namespace Taxi.Helpers
{
    public static class PaginationMetadata
    {
        public static StringValues GeneratePaginationMetadata<T>(PagedList<T> list, PaginationParameters parameters, string prevLink, string nextLink)
        {
            var paginationMetadata = new
            {
                totalCount = list.TotalCount,
                pageSize = list.PageSize,
                currentPage = list.CurrentPage,
                totalPages = list.TotalPages,
                previousPageLink = prevLink,
                nextPageLink = nextLink
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata);
        }

    }
}
