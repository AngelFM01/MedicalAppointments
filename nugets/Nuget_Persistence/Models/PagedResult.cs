using System;
using System.Collections.Generic;

namespace Nuget_Persistence.Models
{
    public class PagedResult<TEntity> where TEntity : class
    {
        public IReadOnlyList<TEntity> Data { get; set; } = Array.Empty<TEntity>();

        public int TotalRecords { get; set; }

        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);
    }
}
